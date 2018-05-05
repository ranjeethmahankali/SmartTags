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
        internal static Dictionary<string, TagButton> TagDict = new Dictionary<string, TagButton>();
        internal static bool UserDeselectFlag = true;
        private static List<Guid> _selectedObjects = new List<Guid>();
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
            if(!TagDict.TryGetValue(name, out tag))
            {
                if (!createNew) { return null; }
                tag = new TagButton(name);
                TagDict.Add(name, tag);
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
            TagDict[name] = btn;
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
            foreach(var key in TagDict.Keys)
            {
                if(TagDict[key].State == TagButtonState.ACTIVE)
                {
                    if (filter == null) { filter = Filter.ParseFromStatement(key); }
                    else { filter = Filter.Combine(filter, Filter.ParseFromStatement(key), Operator.OR); }
                }
            }
            var doc = Rhino.RhinoDoc.ActiveDoc;

            /*
             * When objects are unselected, UI update event is thrown, the user deselect flag needs to indicate if the user is responsible for the
             * deselect event or if its the application.
             */
            UserDeselectFlag = false;
            doc.Objects.UnselectAll();
            UserDeselectFlag = true;

            if (filter != null) { doc.Objects.Select(TagUtil.Evaluate(filter, ref doc)); }
            doc.Views.Redraw();
        }

        public void UpdateSelectedObjectTags(Dictionary<string, List<Guid>> tagObjMap, bool selection, bool updateUI = false)
        {
            foreach(var tag in tagObjMap.Keys)
            {
                TagButton tagBtn;
                if(!TagDict.TryGetValue(tag, out tagBtn)) { continue; }
                if (selection)
                {
                    tagBtn.AddSelectedObjects(tagObjMap[tag]);
                }
                else
                {
                    tagBtn.RemoveSelectedObjects(tagObjMap[tag]);
                    if (UserDeselectFlag)//if a deselection event check the flag
                    {
                        tagBtn.State = TagButtonState.INACTIVE;
                    }
                }
            }

            if (updateUI) { UpdateUI(); }
        }

        public void UpdateAllObjectsDeselected(bool updateUI = false)
        {
            foreach(var tag in TagDict.Keys)
            {
                TagDict[tag].RemoveAllSelectedObjects();
                if (UserDeselectFlag)
                {
                    TagDict[tag].State = TagButtonState.INACTIVE;
                }
            }

            if (UserDeselectFlag) { _selectedObjects = new List<Guid>(); }

            if (updateUI) { UpdateUI(); }
        }

        public void AddToSelectedObjects(IEnumerable<Guid> ids)
        {
            foreach(var id in ids)
            {
                if (!_selectedObjects.Contains(id)) { _selectedObjects.Add(id); }
            }
        }
        public void RemoveFromSelectedObjects(IEnumerable<Guid> ids)
        {
            foreach (var id in ids)
            {
                if (_selectedObjects.Contains(id)) { _selectedObjects.Remove(id); }
            }
        }
        #endregion
    }

    public class TagButton
    {
        #region fields
        private string _tagName;
        private TagButtonState _state;
        List<Guid> _curSelectedObjects = new List<Guid>();
        public Filter CurrentFilter { get; set; }
        #endregion

        #region properties
        public string TagName { get => _tagName; set => _tagName = value; }
        public TagButtonState State { get => _state; set => _state = value; }
        public bool IsObjectSelected { get => _curSelectedObjects.Count > 0; }
        //public List<Guid> CurrentSelectedObjects { get => _curSelectedObjects; }
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
        public void AddSelectedObjects(IEnumerable<Guid> ids)
        {
            _curSelectedObjects = _curSelectedObjects.Union(ids).ToList();
        }
        public void RemoveSelectedObjects(IEnumerable<Guid> ids)
        {
            foreach(var id in ids)
            {
                _curSelectedObjects.Remove(id);
            }
        }
        public void RemoveAllSelectedObjects()
        {
            _curSelectedObjects = new List<Guid>();
        }
        #endregion
    }
}
