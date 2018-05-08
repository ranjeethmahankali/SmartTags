using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using SmartTagsForRhino.Core;

namespace SmartTagsForRhino
{
    public enum TagButtonState { ACTIVE, INACTIVE }
    public enum TagClickState { NORMAL, ADDING, DELETING }

    public partial class Panel_TagManager
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private ContextMenuStrip _contextMenu = null;
        private Label lblCurrentFilter;
        private Panel pnlCurFilter;
        private Label lblCurFilterText;
        private Button btnSaveCurFilter;
        private FlowLayoutPanel pnlSavedFilters;
        private Button btnNewTag;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlTagContainer = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlBody = new System.Windows.Forms.Panel();
            this.pnlTagFilter = new System.Windows.Forms.Panel();
            this.pnlSavedFilters = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlCurFilter = new System.Windows.Forms.Panel();
            this.btnSaveCurFilter = new System.Windows.Forms.Button();
            this.lblCurFilterText = new System.Windows.Forms.Label();
            this.lblCurrentFilter = new System.Windows.Forms.Label();
            this.pnlTitleBar = new System.Windows.Forms.Panel();
            this.btnNewTag = new System.Windows.Forms.Button();
            this.pnlBody.SuspendLayout();
            this.pnlTagFilter.SuspendLayout();
            this.pnlCurFilter.SuspendLayout();
            this.pnlTitleBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTagContainer
            // 
            this.pnlTagContainer.AutoScroll = true;
            this.pnlTagContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTagContainer.Location = new System.Drawing.Point(0, 0);
            this.pnlTagContainer.Name = "pnlTagContainer";
            this.pnlTagContainer.Size = new System.Drawing.Size(326, 377);
            this.pnlTagContainer.TabIndex = 2;
            // 
            // pnlBody
            // 
            this.pnlBody.Controls.Add(this.pnlTagContainer);
            this.pnlBody.Controls.Add(this.pnlTagFilter);
            this.pnlBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBody.Location = new System.Drawing.Point(0, 28);
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Size = new System.Drawing.Size(326, 590);
            this.pnlBody.TabIndex = 3;
            // 
            // pnlTagFilter
            // 
            this.pnlTagFilter.Controls.Add(this.pnlSavedFilters);
            this.pnlTagFilter.Controls.Add(this.pnlCurFilter);
            this.pnlTagFilter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlTagFilter.Location = new System.Drawing.Point(0, 377);
            this.pnlTagFilter.Name = "pnlTagFilter";
            this.pnlTagFilter.Size = new System.Drawing.Size(326, 213);
            this.pnlTagFilter.TabIndex = 3;
            // 
            // pnlSavedFilters
            // 
            this.pnlSavedFilters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSavedFilters.Location = new System.Drawing.Point(0, 85);
            this.pnlSavedFilters.Name = "pnlSavedFilters";
            this.pnlSavedFilters.Size = new System.Drawing.Size(326, 128);
            this.pnlSavedFilters.TabIndex = 2;
            // 
            // pnlCurFilter
            // 
            this.pnlCurFilter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlCurFilter.Controls.Add(this.btnSaveCurFilter);
            this.pnlCurFilter.Controls.Add(this.lblCurFilterText);
            this.pnlCurFilter.Controls.Add(this.lblCurrentFilter);
            this.pnlCurFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCurFilter.Location = new System.Drawing.Point(0, 0);
            this.pnlCurFilter.Name = "pnlCurFilter";
            this.pnlCurFilter.Size = new System.Drawing.Size(326, 85);
            this.pnlCurFilter.TabIndex = 1;
            // 
            // btnSaveCurFilter
            // 
            this.btnSaveCurFilter.Location = new System.Drawing.Point(3, 21);
            this.btnSaveCurFilter.Name = "btnSaveCurFilter";
            this.btnSaveCurFilter.Size = new System.Drawing.Size(84, 23);
            this.btnSaveCurFilter.TabIndex = 2;
            this.btnSaveCurFilter.Text = "Save Filter";
            this.btnSaveCurFilter.UseVisualStyleBackColor = true;
            this.btnSaveCurFilter.Click += new System.EventHandler(this.btnSaveCurFilter_Click);
            // 
            // lblCurFilterText
            // 
            this.lblCurFilterText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCurFilterText.Location = new System.Drawing.Point(94, 0);
            this.lblCurFilterText.MaximumSize = new System.Drawing.Size(326, 0);
            this.lblCurFilterText.Name = "lblCurFilterText";
            this.lblCurFilterText.Size = new System.Drawing.Size(228, 81);
            this.lblCurFilterText.TabIndex = 1;
            this.lblCurFilterText.Text = "None";
            // 
            // lblCurrentFilter
            // 
            this.lblCurrentFilter.AutoSize = true;
            this.lblCurrentFilter.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblCurrentFilter.Location = new System.Drawing.Point(0, 0);
            this.lblCurrentFilter.Name = "lblCurrentFilter";
            this.lblCurrentFilter.Size = new System.Drawing.Size(94, 17);
            this.lblCurrentFilter.TabIndex = 0;
            this.lblCurrentFilter.Text = "Current Filter:";
            // 
            // pnlTitleBar
            // 
            this.pnlTitleBar.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pnlTitleBar.Controls.Add(this.btnNewTag);
            this.pnlTitleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitleBar.Location = new System.Drawing.Point(0, 0);
            this.pnlTitleBar.Name = "pnlTitleBar";
            this.pnlTitleBar.Size = new System.Drawing.Size(326, 28);
            this.pnlTitleBar.TabIndex = 0;
            // 
            // btnNewTag
            // 
            this.btnNewTag.Location = new System.Drawing.Point(5, 1);
            this.btnNewTag.Name = "btnNewTag";
            this.btnNewTag.Size = new System.Drawing.Size(75, 26);
            this.btnNewTag.TabIndex = 0;
            this.btnNewTag.Text = "New Tag";
            this.btnNewTag.UseVisualStyleBackColor = true;
            this.btnNewTag.Click += new System.EventHandler(this.btnNewTag_Click);
            // 
            // Panel_TagManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlTitleBar);
            this.Name = "Panel_TagManager";
            this.Size = new System.Drawing.Size(326, 618);
            this.pnlBody.ResumeLayout(false);
            this.pnlTagFilter.ResumeLayout(false);
            this.pnlCurFilter.ResumeLayout(false);
            this.pnlCurFilter.PerformLayout();
            this.pnlTitleBar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void PnlBody_Resize(object sender, EventArgs e)
        {
            Control body = (Control)sender;
            this.pnlTagFilter.Width = body.Width;
            this.pnlCurFilter.Width = body.Width;
        }

        private Panel pnlTitleBar;
        private FlowLayoutPanel pnlTagContainer;
        private Panel pnlTagFilter;
        private Panel pnlBody;
        #endregion

        #region properties
        internal ContextMenuStrip TagContextMenu
        {
            get
            {
                if(_contextMenu == null)
                {
                    _contextMenu = new ContextMenuStrip();

                    ToolStripMenuItem addTagToSelectedObjs = new ToolStripMenuItem("Add tag to selected object(s)",
                        Properties.Resources.PNG_AddTagIcon, AddTagToSelectedObjsOption_Click);
                    _contextMenu.Items.Add(addTagToSelectedObjs);

                    _contextMenu.Items.Add(new ToolStripSeparator());

                    ToolStripMenuItem deleteTagFromObjs = new ToolStripMenuItem("Delete tag from selected object(s)",
                        Properties.Resources.PNG_RemoveTagIcon, DeleteTagFromObjsOption_Click);
                    _contextMenu.Items.Add(deleteTagFromObjs);

                    ToolStripMenuItem deleteTagFromDoc = new ToolStripMenuItem("Delete tag from document", 
                        Properties.Resources.PNG_DeleteTagIcon, DeleteTagFromDocOption_Click);
                    _contextMenu.Items.Add(deleteTagFromDoc);

                    _contextMenu.Items.Add(new ToolStripSeparator());

                    ToolStripMenuItem filter_AndThis = new ToolStripMenuItem("Filter: AND this", 
                        Properties.Resources.PNG_AndThisFilter, Filter_AndThisOption_Click);
                    _contextMenu.Items.Add(filter_AndThis);

                    ToolStripMenuItem filter_OrThis = new ToolStripMenuItem("Filter: OR this", 
                        Properties.Resources.PNG_OrThisFilter, Filter_OrThisOption_Click);
                    _contextMenu.Items.Add(filter_OrThis);

                    ToolStripMenuItem filter_AndNotThis = new ToolStripMenuItem("Filter: AND NOT this", 
                        Properties.Resources.PNG_AndNotThisFilter, Filter_AndNotThisOption_Click);
                    _contextMenu.Items.Add(filter_AndNotThis);

                    ToolStripMenuItem filter_OrNotThis = new ToolStripMenuItem("Filter: OR NOT this", 
                        Properties.Resources.PNG_OrNotThisFilter, Filter_OrNotThisOption_Click);
                    _contextMenu.Items.Add(filter_OrNotThis);
                }
                return _contextMenu;
            }
        }
        #endregion

        #region methods
        private Button GetNewUITagButton(string tagName, TagButtonState state)
        {
            Button btn = new Button();
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.BorderColor = Color.Blue;
            btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue;
            btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btn.Margin = new System.Windows.Forms.Padding(2);
            btn.Name = "tag_" + tagName;
            btn.AutoSize = true;
            btn.MaximumSize = new Size(100, 30);
            btn.AutoEllipsis = true;

            btn.TabIndex = TagDict.Count + 1;
            btn.Text = tagName;
            btn.UseVisualStyleBackColor = false;

            StyleButton(ref btn, state);
            return btn;
        }

        private void Tag_Click_Toggle(object sender, MouseEventArgs e)
        {
            var clickedBtn = (Button)sender;
            var tagBtn = GetTagButton(clickedBtn.Text, false);
            if(tagBtn == null) { return; }
            tagBtn.FlipState();
            UpdateTag(tagBtn);
            UpdateObjectSelection();
            UpdateUIButton(ref clickedBtn, tagBtn);
        }

        private void AddNewTagButton(TagButton tagBtn)
        {
            var btn = GetNewUITagButton(tagBtn.TagName, tagBtn.State);
            btn.MouseClick += Tag_Click_Toggle;
            btn.ContextMenuStrip = TagContextMenu;
            btn.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
            this.pnlTagContainer.Controls.Add(btn);
        }

        private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ContextMenuStrip strip = sender as ContextMenuStrip;
            if(strip == null) { return; }

            foreach(var menuItem in strip.Items)
            {
                ToolStripMenuItem item = menuItem as ToolStripMenuItem;
                if(item == null) { continue; }
                if (item.Text.ToLower().Contains("selected"))
                {
                    item.Enabled = _selectedObjects != null && _selectedObjects.Count > 0;
                    if (item.Text.ToLower().Contains("delete"))
                    {
                        TagButton tagBtn = GetTagButton(strip.SourceControl.Text, false);
                        item.Enabled = item.Enabled && tagBtn.IsObjectSelected;
                    }
                }
            }

            e.Cancel = false;
        }

        internal void ResetUI()
        {
            //clear all the tag buttons and repopulate the UI
            this.pnlTagContainer.Controls.Clear();
            var tags = TagDict.Keys.ToList();
            tags.Sort();
            foreach(var key in tags)
            {
                AddNewTagButton(TagDict[key]);
            }
            UpdateCurrentFilterText();
            UpdateSavedFilters();
        }

        private void UpdateUI()
        {
            foreach(var control in this.pnlTagContainer.Controls)
            {
                if(!(control is Button)) { continue; }
                var btn = (Button)control;
                TagButton tagBtn;
                if(!TagDict.TryGetValue(btn.Text, out tagBtn)) { continue; }
                UpdateUIButton(ref btn, tagBtn);
            }
            UpdateCurrentFilterText();
            //UpdateSavedFilters();
        }

        public void UpdateSavedFilters()
        {
            this.pnlSavedFilters.Controls.Clear();
            foreach(var filterName in SavedFilters.Keys)
            {
                var btn = GetNewUITagButton(filterName, TagButtonState.INACTIVE);
                btn.Click += FilterSelect_Click;
                this.pnlSavedFilters.Controls.Add(btn);
            }
        }

        private void FilterSelect_Click(object sender, EventArgs e)
        {
            string filterText;
            var filterBtn = sender as Button; 
            if(filterBtn == null) { return; }
            if(!SavedFilters.TryGetValue(filterBtn.Text, out filterText)) { return; }
            //apply filter
            CurrentFilter = Filter.ParseFromStatement(filterText);
            ApplyCurrentFilter(false);
            UpdateCurrentFilterText();
        }

        private void UpdateUIButton(ref Button btn, TagButton tagBtn)
        {
            btn.Text = tagBtn.TagName;
            StyleButton(ref btn, tagBtn.State);
            btn.FlatAppearance.BorderSize = tagBtn.IsObjectSelected ? 2 : 0;
        }

        public void UpdateCurrentFilterText()
        {
            this.lblCurFilterText.Text = CurrentFilter == null ? "None" : CurrentFilter.ToString();
        }

        private void StyleButton(ref Button btn, TagButtonState state)
        {
            if (state == TagButtonState.ACTIVE)
            {
                btn.BackColor = Color.Blue;
                btn.ForeColor = Color.White;
            }
            else
            {
                btn.BackColor = System.Drawing.SystemColors.ScrollBar;
                btn.ForeColor = Color.Black;
            }
        }

        private Button GetUIButton(int index)
        {
            if (index >= this.pnlTagContainer.Controls.Count || !(this.pnlTagContainer.Controls[index] is Button)) { return null; }
            return this.pnlTagContainer.Controls[index] as Button;
        }

        //various context menu option event handlers
        private void Filter_OrNotThisOption_Click(object sender, EventArgs e)
        {
            string tag = ((sender as ToolStripMenuItem)?.Owner as ContextMenuStrip)?.SourceControl?.Text;
            if (tag == null) { return; }
            Filter notThisFilter = Filter.Invert(Filter.ParseFromStatement(tag));
            CurrentFilter = CurrentFilter == null ? notThisFilter : Filter.Combine(CurrentFilter, notThisFilter, Operator.OR);
            ApplyCurrentFilter();
        }

        private void Filter_AndNotThisOption_Click(object sender, EventArgs e)
        {
            string tag = ((sender as ToolStripMenuItem)?.Owner as ContextMenuStrip)?.SourceControl?.Text;
            if (tag == null) { return; }
            Filter notThisFilter = Filter.Invert(Filter.ParseFromStatement(tag));
            CurrentFilter = CurrentFilter == null ? notThisFilter : Filter.Combine(CurrentFilter, notThisFilter, Operator.AND);
            ApplyCurrentFilter();
        }

        private void Filter_OrThisOption_Click(object sender, EventArgs e)
        {
            string tag = ((sender as ToolStripMenuItem)?.Owner as ContextMenuStrip)?.SourceControl?.Text;
            if (tag == null) { return; }
            Filter thisFilter = Filter.ParseFromStatement(tag);
            CurrentFilter = CurrentFilter == null ? thisFilter : Filter.Combine(CurrentFilter, thisFilter, Operator.OR);
            ApplyCurrentFilter();
        }

        private void Filter_AndThisOption_Click(object sender, EventArgs e)
        {
            string tag = ((sender as ToolStripMenuItem)?.Owner as ContextMenuStrip)?.SourceControl?.Text;
            if (tag == null) { return; }
            Filter thisFilter = Filter.ParseFromStatement(tag);
            CurrentFilter = CurrentFilter == null ? thisFilter : Filter.Combine(CurrentFilter, thisFilter, Operator.AND);
            ApplyCurrentFilter();
        }

        private void DeleteTagFromDocOption_Click(object sender, EventArgs e)
        {
            string tag = ((sender as ToolStripMenuItem)?.Owner as ContextMenuStrip)?.SourceControl?.Text;
            if (tag == null) { return; }

            Rhino.RhinoApp.RunScript(string.Format("{0} {1}", Commands.TagFilterCommand.CommandString, tag), false);
            Rhino.RhinoApp.RunScript(string.Format("{0} {1}", Commands.DeleteTagsCommand.CommandString, tag), true);
            ResetUI();
        }

        private void DeleteTagFromObjsOption_Click(object sender, EventArgs e)
        {
            string tag = ((sender as ToolStripMenuItem)?.Owner as ContextMenuStrip)?.SourceControl?.Text;
            if (tag == null || _selectedObjects.Count == 0) { return; }

            Rhino.RhinoApp.RunScript(string.Format("{0} {1}", Commands.DeleteTagsCommand.CommandString, tag), true);
        }

        private void AddTagToSelectedObjsOption_Click(object sender, EventArgs e)
        {
            string tag = ((sender as ToolStripMenuItem)?.Owner as ContextMenuStrip)?.SourceControl?.Text;
            if(tag == null || _selectedObjects.Count == 0) { return; }

            Rhino.RhinoApp.RunScript(string.Format("{0} {1}", Commands.TagCommand.CommandString, tag), true);
        }

        private void btnSaveCurFilter_Click(object sender, EventArgs e)
        {
            Rhino.RhinoApp.RunScript(Commands.SaveCurrentFilter.CommandString, true);
        }

        private void btnNewTag_Click(object sender, EventArgs e)
        {
            Rhino.RhinoApp.RunScript(Commands.TagCommand.CommandString, true);
        }
        #endregion
    }
}
