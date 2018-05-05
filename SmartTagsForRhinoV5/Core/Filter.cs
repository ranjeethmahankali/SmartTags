using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.DocObjects;

namespace SmartTagsForRhino.Core
{
    public class Filter
    {
        #region-fields
        private string _filterText = null;
        private Predicate<RhinoObject> _testObject;
        #endregion

        #region-properties
        public Predicate<RhinoObject> TestObject
        {
            get
            {
                //by default no object can pass through the filter if it ever comes to this.
                if(_testObject == null) { _testObject = (r) => false; }
                return _testObject;
            }
        }
        #endregion

        #region-constructors
        private Filter(Predicate<RhinoObject> test)
        {
            _testObject = test;
        }
        #endregion

        #region-methods
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
        public static Filter ParseFromStatement(string filterStatement)
        {
            string errMsg = "Something about this filter is not correct! Please check to make sure its formatted correctly.";
            List<string> terms = SplitStatement(filterStatement);
            if (terms.Count < 1)
            {
                throw new SyntaxException(errMsg);
            }
            if (terms.Count == 1)
            {
                Filter f = new Filter((o) =>
                {
                    var tags = TagUtil.GetTags(o);
                    return tags.Contains(terms[0]);
                });
                f._filterText = terms[0];
                return f;
            }

            Filter filter;
            int i;
            if (Operator.ByName(terms[0]) == Operator.NOT)
            {
                filter = Invert(ParseFromStatement(terms[1]));
                i = 2;
            }
            else
            {
                filter = ParseFromStatement(terms[0]);
                i = 1;
            }

            try
            {
                while (i < terms.Count - 1)
                {
                    if(!Operator.IsOperator(terms[i]) || Operator.ByName(terms[i]) == Operator.NOT)
                    {
                        throw new SyntaxException(errMsg);
                    }
                    var op = Operator.ByName(terms[i]);
                    if(Operator.ByName(terms[i+1]) == Operator.NOT)
                    {
                        filter = Combine(filter, Invert(ParseFromStatement(terms[i + 2])), op);
                        i += 3;
                    }
                    else
                    {
                        filter = Combine(filter, ParseFromStatement(terms[i + 1]), op);
                        i += 2;
                    }
                }
            }
            catch(IndexOutOfRangeException e)
            {
                throw new SyntaxException(errMsg + ":\n" + e.Message);
            }
            return filter;
        }
        public static Filter Invert(Filter filter)
        {
            Filter f = new Filter((rhObj) =>{
                return !filter.TestObject.Invoke(rhObj);
            });
            f._filterText = string.Format("(not {0})", filter._filterText);
            return f;
        }
        public static Filter Combine(Filter f1, Filter f2, Operator op)
        {
            Filter fResult;
            if(op == Operator.AND)
            {
                fResult = new Filter((rhObj) => {
                    return f1.TestObject.Invoke(rhObj) && f2.TestObject.Invoke(rhObj);
                });
            }
            else if (op == Operator.OR)
            {
                fResult = new Filter((rhObj) => {
                    return f1.TestObject.Invoke(rhObj) || f2.TestObject.Invoke(rhObj);
                });
            }
            else
            {
                throw new InvalidOperationException(string.Format("Cannot apply {0} operator for 2 filters.", op.ToString()));
            }

            fResult._filterText = string.Format("({0} {1} {2})", f1._filterText, op.Name, f2._filterText);
            return fResult;
        }

        public override string ToString()
        {
            return _filterText ?? "";
        }
        #endregion
    }
}
