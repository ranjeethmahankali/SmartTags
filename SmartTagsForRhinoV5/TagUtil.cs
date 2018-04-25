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
    public class SyntaxException : Exception
    {
        public SyntaxException(string message) : base(message) { }
    }

    //static class with tag related functions
    public static class TagUtil
    {
        public static string appKey = "smartTagData";
        //these words cannot be used
        public static List<string> RESERVED = new List<string>(Operator.Names);
        public static List<char> BAD_CHARS = new List<char>(new char[] { ',', '.', ' '});
        //this dictionary will contail all the tags
        //this dictionary will be used as a rough column while solving a filter statement

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
            string jsonStr = obj.Attributes.GetUserString(appKey);
            if (jsonStr == "" || jsonStr == null)
            {
                return new List<string>();
            }

            return JsonConvert.DeserializeObject<List<string>>(jsonStr);
        }

        //saves the list of tags in the object attributes as a jsonString
        public static void SaveTags(RhinoObject obj, List<string> tagList)
        {
            string jsonStr = JsonConvert.SerializeObject(tagList);
            obj.Attributes.SetUserString(appKey, jsonStr);
        }

        //adds new tags with objects, or new objects to old tags
        public static void AddTag(RhinoObject obj, string tag)
        {
            if (!IsValid(tag)) { return; }
            List<string> tags = GetTags(obj);
            if (tags.Contains(tag)) { return; }
            tags.Add(tag);
            SaveTags(obj, tags);
        }
        //deletes existing tags. returns true if all goes well or else returs false
        public static bool DeleteTag(RhinoObject obj, string tag)
        {
            List<string> tags = GetTags(obj);
            if (!tags.Contains(tag)) { return false; }
            tags.Remove(tag);
            SaveTags(obj, tags);
            return true;
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