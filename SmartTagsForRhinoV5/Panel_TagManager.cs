using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartTagsForRhino.Core;

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
        /// <summary>
        /// Either returns an exisiting TagButton object with the given name, or creates a new one and adds it to the dictionary
        /// </summary>
        /// <param name="name">name of the tag</param>
        /// <param name="createNew">
        /// Set this to false if you don't want to create a new tag in case one with the 
        /// provided name doesn't already exist, in which case, null will be returned;
        /// </param>
        /// <returns>
        /// Returns an exisitng tag button, or if one doesn't exist with the given name creates a new one, 
        /// adds it to the dicionary and returns it. If createNew is set to false and a tag with the name cannot be found, null is returned.
        /// </returns>
        public TagButton GetTagButton(string name, bool createNew = true)
        {
            TagButton tag;
            if(!_tagDict.TryGetValue(name, out tag))
            {
                if (!createNew) { return null; }
                tag = new TagButton(name);
                _tagDict.Add(name, tag);
            }
            return tag;
        }
        /// <summary>
        /// updates the state of an individual tag
        /// </summary>
        /// <param name="name">name of the tag to be updated</param>
        /// <param name="state">state of the tag</param>
        /// <param name="updateUI">specify whether or not to update the UI to reflect this update</param>
        public void UpdateTag(string name, TagButtonState state = TagButtonState.INACTIVE, bool updateUI = false)
        {
            TagButton btn = GetTagButton(name);
            btn.State = state;
            _tagDict[name] = btn;
            if (updateUI) { ResetUI(); }
        }
        public void UpdateTag(TagButton tagBtn, bool updateUI = false)
        {
            UpdateTag(tagBtn.TagName, tagBtn.State, updateUI);
        }
        /// <summary>
        /// If any of the tags are currently active, this function selects the objects that have those tags
        /// </summary>
        public void UpdateObjectSelection()
        {
            Filter filter = null;
            foreach(var key in _tagDict.Keys)
            {
                if(_tagDict[key].State == TagButtonState.ACTIVE)
                {
                    if (filter == null) { filter = Filter.ParseFromStatement(key); }
                    else { filter = Filter.Combine(filter, Filter.ParseFromStatement(key), Operator.OR); }
                }
            }
            var doc = Rhino.RhinoDoc.ActiveDoc;
            doc.Objects.UnselectAll();
            if (filter != null) { doc.Objects.Select(TagUtil.Evaluate(filter, ref doc)); }
            doc.Views.Redraw();
        }

        public void UpdateSelectedObjectTags(List<string> tags, bool selection, bool updateUI = false)
        {
            foreach(var tag in tags)
            {
                TagButton tagBtn;
                if(!_tagDict.TryGetValue(tag, out tagBtn)) { continue; }
                tagBtn.IsObjectSelected = selection;
            }

            if (updateUI) { UpdateUI(); }
        }

        public void UpdateAllObjectsDeselected(bool updateUI = false)
        {
            foreach(var tag in _tagDict.Keys)
            {
                _tagDict[tag].IsObjectSelected = false;
            }

            if (updateUI) { UpdateUI(); }
        }
        #endregion
    }

    public class TagButton
    {
        #region fields
        private string _tagName;
        private TagButtonState _state;
        private bool _isObjectSelected = false;
        #endregion

        #region properties
        public string TagName { get => _tagName; set => _tagName = value; }
        public TagButtonState State { get => _state; set => _state = value; }
        public bool IsObjectSelected { get => _isObjectSelected; set => _isObjectSelected = value; }
        #endregion

        #region constructors
        internal TagButton(string name, TagButtonState state)
        {
            _tagName = name;
            _state = state;
        }
        internal TagButton(string name): this(name, TagButtonState.INACTIVE) { }
        #endregion

        #region methods
        public void FlipState()
        {
            _state = _state == TagButtonState.ACTIVE ? TagButtonState.INACTIVE : TagButtonState.ACTIVE;
        }
        #endregion
    }
}
