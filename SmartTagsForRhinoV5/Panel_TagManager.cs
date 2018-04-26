using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartTagsForRhino
{
    //TODO: Create your own Guid for this class
    [System.Runtime.InteropServices.Guid("01EF7393-4EF9-453D-AB70-C3E97B150195")]
    public partial class Panel_TagManager : UserControl
    {
        #region fields
        private Dictionary<string, TagButton> _tagDict = new Dictionary<string, TagButton>();
        #endregion

        #region constructors
        public Panel_TagManager()
        {
            InitializeComponent();
        }
        #endregion

        #region methods

        public TagButton GetTagButton(string name)
        {
            TagButton tag;
            if(!_tagDict.TryGetValue(name, out tag))
            {
                tag = new TagButton(name);
                _tagDict.Add(name, tag);
            }
            return tag;
        }

        public void UpdateTag(string name, TagButtonState state = TagButtonState.INACTIVE)
        {
            TagButton btn = GetTagButton(name);
            btn.State = state;
            _tagDict[name] = btn;
            UpdateUI();
        }
        #endregion
    }

    public class TagButton
    {
        #region fields
        private string _tagName;
        private TagButtonState _state;
        #endregion

        #region properties
        public string TagName { get => _tagName; set => _tagName = value; }
        public TagButtonState State { get => _state; set => _state = value; }
        #endregion

        #region constructors
        internal TagButton(string name, TagButtonState state)
        {
            _tagName = name;
            _state = state;
        }
        internal TagButton(string name): this(name, TagButtonState.INACTIVE) { }
        #endregion
    }
}
