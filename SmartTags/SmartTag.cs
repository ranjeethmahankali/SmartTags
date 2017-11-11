using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Rhino;
using System.Diagnostics;
using Rhino.DocObjects;

namespace SmartTags
{
    //exception to throw when user's filter statement syntax is wrong
    public class SyntaxException : Exception
    {
        public SyntaxException(string message) : base(message) { }
    }
    //static class with tag related functions
    public static class SmartTag
    {
        public static string appKey = "smartTagData";
        public static string AND = "and";
        public static string OR = "or";
        public static string NOT = "not";
        //these words cannot be used
        public static List<string> RESERVED = new List<string>(new string[] { AND, OR });
        public static List<string> BAD_CHARS = new List<string>(new string[] { ",", ".", " " });
        //this dictionary will contail all the tags
        //this dictionary will be used as a rough column while solving a filter statement

        //returns if a word is valid for use as a tag
        public static bool IsValid(string word)
        {
            if (RESERVED.Contains(word)) { return false; }

            foreach (string illegal in BAD_CHARS)
            {
                if (word.Contains(illegal)) { return false; }
            }

            //if we made it till here then that means the word was fine.
            return true;
        }

        //gets the list of tags associated with an object
        public static List<string> getTags(RhinoObject obj)
        {
            string jsonStr = obj.Attributes.GetUserString(appKey);
            if (jsonStr == "" || jsonStr == null)
            {
                return new List<string>();
            }

            return JsonConvert.DeserializeObject<List<string>>(jsonStr);
        }

        //saves the list of tags in the object attributes as a jsonString
        public static void saveTags(RhinoObject obj, List<string> tagList)
        {
            string jsonStr = JsonConvert.SerializeObject(tagList);
            obj.Attributes.SetUserString(appKey, jsonStr);
        }

        //adds new tags with objects, or new objects to old tags
        public static void AddTag(RhinoObject obj, string tag)
        {
            if (!IsValid(tag)) { return; }
            List<string> tags = getTags(obj);
            if (tags.Contains(tag)) { return; }
            tags.Add(tag);
            saveTags(obj, tags);
        }
        //deletes existing tags. returns true if all goes well or else returs false
        public static bool DeleteTag(RhinoObject obj, string tag)
        {
            List<string> tags = getTags(obj);
            if (!tags.Contains(tag)) { return false; }
            tags.Remove(tag);
            saveTags(obj, tags);
            return true;
        }

        //this checks if a statement or a substatement is valid
        private static bool statementIsValid(string statement)
        {
            int openBraces = statement.Count(f => f == '(');
            int closingBraces = statement.Count(f => f == ')');
            if (openBraces != closingBraces)
            {
                string msg = "Please check the use of parenthesis in the filter statement";
                Rhino.UI.Dialogs.ShowMessageBox(msg, "Warning");
                return false;
            }


            return true;
        }

        //this splits a statement into terms
        private static List<string> SplitStatement(string statement)
        {
            statement = statement.Trim(' ');
            List<string> terms = new List<string>();
            Stack<int> braces = new Stack<int>();
            int lastPos = 0;
            for (int i = 0; i < statement.Length; i++)
            {
                char ch = statement[i];
                if ((ch == ' ' || ch == '(' || i == statement.Length - 1) && braces.Count == 0)
                {
                    int endPos = (i == statement.Length - 1) ? i + 1 : i;
                    string subStr = statement.Substring(lastPos, endPos - lastPos);
                    subStr = subStr.Trim(' ');
                    if (subStr.Length > 0) terms.Add(subStr);
                    lastPos = i + 1;
                }

                if (ch == '(') { braces.Push(i); }
                else if (ch == ')')
                {
                    int open = braces.Pop();
                    if (braces.Count == 0)
                    {
                        string subStr = statement.Substring(open + 1, i - open - 1);
                        subStr = subStr.Trim(' ');
                        if (subStr.Length > 0) terms.Add(subStr);
                        lastPos = i + 1;
                    }
                }
            }
            return terms;
        }

        //this performs boolean operations on sets
        private static List<Guid> BooleanOperation(List<Guid> set1, List<Guid> set2, string operation)
        {
            if (operation == AND) { return set1.Intersect(set2).ToList(); }
            else if (operation == OR) { return set1.Union(set2).ToList(); }
            else
            {
                string msg = string.Format("Boolean operator {0} not recognized", operation);
                Rhino.UI.Dialogs.ShowMessageBox(msg, "Warning");
                return new List<Guid>();
            }
        }

        private static bool PassesFilter(RhinoObject obj, string filterStatement)
        {
            string errorMsg = "something was not right with that filter statement! please check it.";
            bool passed;
            List<string> tags = getTags(obj);
            if (tags.Contains(filterStatement)) { return true; }

            List<string> terms = SplitStatement(filterStatement);
            if (terms.Count < 1) { return false; }
            else if (terms.Count == 1) { return tags.Contains(terms[0]); }

            int i;
            if (terms[0] == NOT) { passed = !PassesFilter(obj, terms[1]); i = 2; }
            else { passed = PassesFilter(obj, terms[0]); i = 1; }

            try
            {
                while (i < terms.Count - 1)
                {
                    if (terms[i] == AND)
                    {
                        if (terms[i + 1] == NOT)
                        {
                            passed = passed && !(PassesFilter(obj, terms[i + 2]));
                            i += 3;
                        }
                        else
                        {
                            passed = passed && PassesFilter(obj, terms[i + 1]);
                            i += 2;
                        }
                    }
                    else if (terms[i] == OR)
                    {
                        if (terms[i + 1] == NOT)
                        {
                            passed = passed || !(PassesFilter(obj, terms[i + 2]));
                            i += 3;
                        }
                        else
                        {
                            passed = passed || PassesFilter(obj, terms[i + 1]);
                            i += 2;
                        }
                    }
                    else { throw new SyntaxException(errorMsg); }
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new SyntaxException(errorMsg);
            }
            return passed;
        }
        public static List<Guid> Evaluate(string filterStatement, ref RhinoDoc doc)
        {
            if (!statementIsValid(filterStatement))
            {
                Rhino.UI.Dialogs.ShowMessageBox("Check the syntax of the filter statement!", "Warning");
                return null;
            }
            List<Guid> ids = new List<Guid>();
            ObjectEnumeratorSettings settings = new ObjectEnumeratorSettings();
            IEnumerator<RhinoObject> objList = doc.Objects.GetEnumerator();
            try
            {
                while (objList.MoveNext())
                {
                    RhinoObject obj = objList.Current;
                    if (PassesFilter(obj, filterStatement)) { ids.Add(obj.Id); }
                }
            }
            catch (SyntaxException e)
            {
                Rhino.UI.Dialogs.ShowMessageBox(e.Message, "Operation Failed");
            }

            return ids;
        }
    }
}