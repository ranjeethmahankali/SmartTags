using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Rhino;
using System.Diagnostics;
using Rhino.DocObjects;
using SmartTagsForRhino.Core;

namespace SmartTagsForRhino
{
    //exception to throw when user's filter statement syntax is wrong
    [Serializable]
    public class SyntaxException : Exception
    {
        public SyntaxException(string message) : base(message) { }
    }

    //static class with tag related functions
    public static class TagUtil
    {
        public static string AppKey = "smartTagData_791f4b8c-e0c3-4d3e-b4ba-098a937a8bd2";
        //these words cannot be used
        public static List<string> RESERVED = new List<string>(Operator.Names);
        public static List<char> BAD_CHARS = new List<char>(new char[] { ',', '.', ' '});

        public static RhinoDoc ActiveDocument { get; set; }

        public static Panel_TagManager TagManager
        {
            get
            {
#if RhinoV5
                return Rhino.UI.Panels.GetPanel(typeof(Panel_TagManager).GUID) as Panel_TagManager;
#elif RhinoV6
                return Rhino.UI.Panels.GetPanel<Panel_TagManager>(ActiveDocument) as Panel_TagManager;
#endif
            }
        }

        //update the tag manager panel UI with new tags
        public static void UpdateUI(string tag, TagButtonState state = TagButtonState.INACTIVE, bool updateUI = true)
        {
            TagManager?.UpdateTag(tag, state , updateUI);
        }

        //returns if a word is valid for use as a tag
        public static bool IsValid(string word)
        {
            if (RESERVED.Contains(word)) { return false; }

            foreach (char illegal in BAD_CHARS)
            {
                if (word.Contains(illegal)) { return false; }
            }

            //if we made it till here then that means the word was fine.
            return true;
        }

        //gets the list of tags associated with an object
        public static List<string> GetTags(RhinoObject obj)
        {
            string jsonStr = obj.Attributes.GetUserString(AppKey);
            if (jsonStr == "" || jsonStr == null)
            {
                return new List<string>();
            }

            return JsonConvert.DeserializeObject<List<string>>(jsonStr);
        }

        //gets the list of tags associated with the given objects (union of all tags of all objects)
        public static List<string> GetTagsUnion(IEnumerable<RhinoObject> objs)
        {
            IEnumerable<string> tags = new List<string>();
            foreach(var obj in objs)
            {
                tags = tags.Union(GetTags(obj));
            }

            return tags.ToList();
        }

        //returns the tags of the given objects in a guid map
        public static Dictionary<string, List<Guid>> GetTagUidMap(IEnumerable<RhinoObject> objs)
        {
            Dictionary<string, List<Guid>> dict = new Dictionary<string, List<Guid>>();
            foreach(var obj in objs)
            {
                List<string> tags = GetTags(obj);
                foreach(var tag in tags)
                {
                    if (!dict.ContainsKey(tag))
                    {
                        dict.Add(tag, new List<Guid>() { obj.Id });
                    }
                    else { dict[tag].Add(obj.Id); }
                }
            }

            return dict;
        }

        //saves the list of tags in the object attributes as a jsonString
        public static void SaveTags(RhinoObject obj, List<string> tagList)
        {
            string jsonStr = JsonConvert.SerializeObject(tagList);
            obj.Attributes.SetUserString(AppKey, jsonStr);
        }

        //adds new tags to objects, or new objects to old tags
        public static bool AddTag(RhinoObject obj, string tag, bool updateUI = false)
        {
            if (!IsValid(tag))
            {
#if RhinoV5
                Rhino.UI.Dialogs.ShowMessageBox("Please make sure that tag names only contain alpha numeric characters", "Tag names are invalid");
#elif RhinoV6
                Rhino.UI.Dialogs.ShowMessage("Please make sure that tag names only contain alpha numeric characters", "Tag names are invalid");
#endif
                return false;
            }
            List<string> tags = GetTags(obj);
            if (tags.Contains(tag)) { return true; }
            tags.Add(tag);
            SaveTags(obj, tags);

            if (updateUI) { UpdateUI(tag); }
            return true;
        }
        //adds new tags with objects, or new objects to old tags
        public static bool AddTag(IEnumerable<RhinoObject> objs, string tag, bool updateUI = false)
        {
            bool success = true;
            foreach(var obj in objs)
            {
                if(!AddTag(obj, tag, updateUI))
                {
                    success = false;
                    break;
                }
            }
            //if (updateUI) { UpdateUI(tag); }
            return success;
        }
        //deletes existing tags. returns true if all goes well or else returs false
        public static void DeleteTag(ref RhinoObject obj, string tag, bool updateUI = false)
        {
            List<string> tags = GetTags(obj);
            if (!tags.Contains(tag)) { return; }
            tags.Remove(tag);
            SaveTags(obj, tags);

            if (updateUI) { UpdateUI(tag); }
        }
        //deletes existing tags. returns true if all goes well or else returs false
        public static void DeleteTag(ref List<RhinoObject> objs, string tag, bool updateUI = false)
        {
            for(int i = 0; i <objs.Count; i++)
            {
                var obj = objs[i];
                DeleteTag(ref obj, tag);
                objs[i] = obj;
            }
            if (updateUI) { UpdateUI(tag); }
        }

        //this checks if a statement or a substatement is valid
        private static bool StatementIsValid(string statement)
        {
            int openBraces = statement.Count(f => f == '(');
            int closingBraces = statement.Count(f => f == ')');
            if (openBraces != closingBraces)
            {
                string msg = "Please check the use of parenthesis in the filter statement";
                ShowMessage(msg, "Warning");
                return false;
            }


            return true;
        }

        //this performs boolean operations on sets
        private static List<Guid> BooleanOperation(List<Guid> set1, List<Guid> set2, Operator op)
        {
            if (op == Operator.AND) { return set1.Intersect(set2).ToList(); }
            else if (op == Operator.OR) { return set1.Union(set2).ToList(); }
            else
            {
                string msg = string.Format("Cannot apply {0} operator on two sets", op.Name);
                ShowMessage(msg, "Warning");
                return new List<Guid>();
            }
        }

        public static List<Guid> Evaluate(string filterStatement, ref RhinoDoc doc)
        {
            if (!StatementIsValid(filterStatement))
            {
                ShowMessage("The filter statement contains Illegal words or characters.", "Warning");
                return null;
            }
            Filter filter = null;
            try
            {
                filter = Filter.ParseFromStatement(filterStatement);
            }
            catch(SyntaxException e)
            {
                ShowMessage(e.Message, "Operation Failed");
            }
            return filter == null ? new List<Guid>() : Evaluate(filter, ref doc);
        }

        public static List<Guid> Evaluate(Filter filter, ref RhinoDoc doc)
        {
            List<Guid> ids = new List<Guid>();
            //ObjectEnumeratorSettings settings = new ObjectEnumeratorSettings();
            IEnumerator<RhinoObject> objList = doc.Objects.GetEnumerator();
            while (objList.MoveNext())
            {
                RhinoObject obj = objList.Current;
                if (filter.TestObject(obj)) { ids.Add(obj.Id); }
            }
            return ids;
        }

        public static List<string> GetAllTags(RhinoDoc doc)
        {
            ActiveDocument = doc;
            IEnumerable<string> tags = new List<string>();
            if(doc == null) { return tags.ToList(); }
            IEnumerator<RhinoObject> objList = doc.Objects.GetEnumerator();
            while (objList.MoveNext())
            {
                RhinoObject obj = objList.Current;
                tags = tags.Union(GetTags(obj));
            }

            return tags.ToList();
        }

        public static void SetCurrentDocumentTags(List<string> tags, bool merge = false)
        {
            if (!merge)
            {
                Panel_TagManager.TagDict = new Dictionary<string, TagButton>();
            }
            foreach(string tag in tags)
            {
                TagButton btn = new TagButton(tag, TagButtonState.INACTIVE);
                if (Panel_TagManager.TagDict.ContainsKey(tag))
                {
                    Panel_TagManager.TagDict[tag] = btn;
                }
                else { Panel_TagManager.TagDict.Add(tag, btn); }
            }
            //updating the UI if the panel is loaded
            TagManager?.ResetUI();
        }

        public static List<string> GetCurrentDocumentTags()
        {
            return Panel_TagManager.TagDict.Keys.Select((s) => (string)s.Clone()).ToList();
        }

        public static void AddSavedFiltersToDocument(RhinoDoc doc = null)
        {
            string jsonStr = JsonConvert.SerializeObject(Panel_TagManager.SavedFilters);
            (doc??ActiveDocument)?.Strings.SetString(AppKey, jsonStr);
            TagManager?.UpdateSavedFilters();
        }

        public static void LoadSavedFiltersFromDocument(RhinoDoc doc = null)
        {
            string jsonStr = (doc ?? ActiveDocument)?.Strings.GetValue(AppKey);
            if(jsonStr == null) { Panel_TagManager.SavedFilters = new Dictionary<string, string>(); }
            else
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr);
                if(dict != null) { Panel_TagManager.SavedFilters = dict; }
            }
            TagManager?.UpdateSavedFilters();
        }

#if RhinoV5
        public static System.Windows.Forms.DialogResult ShowMessage(string message, string title)
        {
            return Rhino.UI.Dialogs.ShowMessageBox(message, title);
        }
#elif RhinoV6
        public static Rhino.UI.ShowMessageResult ShowMessage(string message, string title)
        {
            return Rhino.UI.Dialogs.ShowMessage(message, title);
        }
#endif
    }
}