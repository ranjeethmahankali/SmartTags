using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTagsForRhino
{
    public class Operator
    {
        #region-fields
        private static Dictionary<string, Operator> _operatorDict = new Dictionary<string, Operator>();

        private string _name;
        #endregion

        #region-properties
        public static List<string> Names
        {
            get { return _operatorDict.Keys.ToList(); }
        }
        public string Name
        {
            get { return _name; }
        }
        #endregion

        #region-constructors
        private Operator(string name)
        {
            _name = name;
            _operatorDict.Add(_name, this);
        }
        #endregion

        #region-methods
        public static Operator ByName(string name)
        {
            Operator op;
            if (_operatorDict.TryGetValue(name.ToLower(), out op)) { return op; }
            else { return null; }
        }
        public static bool IsOperator(string name)
        {
            return _operatorDict.ContainsKey(name.ToLower());
        }
        #endregion

        #region-static members
        public static readonly Operator AND = new Operator("and"),
            OR = new Operator("or"),
            NOT = new Operator("not");
        #endregion
    }
}
