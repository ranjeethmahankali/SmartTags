using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
            this.pnlTitleBar = new System.Windows.Forms.Panel();
            this.pnlTagFilter = new System.Windows.Forms.Panel();
            this.pnlBody = new System.Windows.Forms.Panel();
            this.pnlBody.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTagContainer
            // 
            this.pnlTagContainer.AutoScroll = true;
            this.pnlTagContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTagContainer.Location = new System.Drawing.Point(0, 0);
            this.pnlTagContainer.Name = "pnlTagContainer";
            this.pnlTagContainer.Size = new System.Drawing.Size(326, 459);
            this.pnlTagContainer.TabIndex = 2;
            // 
            // pnlTitleBar
            // 
            this.pnlTitleBar.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pnlTitleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitleBar.Location = new System.Drawing.Point(0, 0);
            this.pnlTitleBar.Name = "pnlTitleBar";
            this.pnlTitleBar.Size = new System.Drawing.Size(326, 28);
            this.pnlTitleBar.TabIndex = 0;
            // 
            // pnlTagFilter
            // 
            this.pnlTagFilter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlTagFilter.Location = new System.Drawing.Point(0, 459);
            this.pnlTagFilter.Name = "pnlTagFilter";
            this.pnlTagFilter.Size = new System.Drawing.Size(326, 131);
            this.pnlTagFilter.TabIndex = 3;
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
            // Panel_TagManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlTitleBar);
            this.Name = "Panel_TagManager";
            this.Size = new System.Drawing.Size(326, 618);
            this.pnlBody.ResumeLayout(false);
            this.ResumeLayout(false);
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

                    ToolStripMenuItem addTagToSelectedObjs = new ToolStripMenuItem("Add tag to selected object(s)");
                    addTagToSelectedObjs.Click += AddTagToSelectedObjsOption_Click;
                    _contextMenu.Items.Add(addTagToSelectedObjs);

                    _contextMenu.Items.Add(new ToolStripSeparator());

                    ToolStripMenuItem deleteTagFromObjs = new ToolStripMenuItem("Delete tag from selected object(s)");
                    deleteTagFromObjs.Click += DeleteTagFromObjsOption_Click;
                    _contextMenu.Items.Add(deleteTagFromObjs);

                    ToolStripMenuItem deleteTagFromDoc = new ToolStripMenuItem("Delete tag from document");
                    deleteTagFromDoc.Click += DeleteTagFromDocOption_Click;
                    _contextMenu.Items.Add(deleteTagFromDoc);

                    _contextMenu.Items.Add(new ToolStripSeparator());

                    ToolStripMenuItem filter_AndThis = new ToolStripMenuItem("Filter: AND this");
                    filter_AndThis.Click += Filter_AndThisOption_Click;
                    _contextMenu.Items.Add(filter_AndThis);

                    ToolStripMenuItem filter_OrThis = new ToolStripMenuItem("Filter: OR this");
                    filter_OrThis.Click += Filter_OrThisOption_Click;
                    _contextMenu.Items.Add(filter_OrThis);

                    ToolStripMenuItem filter_AndNotThis = new ToolStripMenuItem("Filter: AND NOT this");
                    filter_AndNotThis.Click += Filter_AndNotThisOption_Click;
                    _contextMenu.Items.Add(filter_AndNotThis);

                    ToolStripMenuItem filter_OrNotThis = new ToolStripMenuItem("Filter: OR NOT this");
                    filter_OrNotThis.Click += Filter_OrNotThisOption_Click;
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
            btn.MouseClick += Tag_Click_Toggle;

            btn.TabIndex = TagDict.Count + 1;
            btn.Text = tagName;
            btn.UseVisualStyleBackColor = false;

            //btn.ContextMenu = new ContextMenu(new MenuItem[] { new MenuItem("One"), new MenuItem("Two"), new MenuItem("Three",
            //    new MenuItem[] { new MenuItem("A"), new MenuItem("B"), new MenuItem("C") }) });

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
            //clear everything and repopulate the UI
            this.pnlTagContainer.Controls.Clear();
            var tags = TagDict.Keys.ToList();
            tags.Sort();
            foreach(var key in tags)
            {
                AddNewTagButton(TagDict[key]);
            }
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
        }

        private void UpdateUIButton(ref Button btn, TagButton tagBtn)
        {
            btn.Text = tagBtn.TagName;
            StyleButton(ref btn, tagBtn.State);
            btn.FlatAppearance.BorderSize = tagBtn.IsObjectSelected ? 2 : 0;
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
            throw new NotImplementedException();
        }

        private void Filter_AndNotThisOption_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Filter_OrThisOption_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Filter_AndThisOption_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DeleteTagFromDocOption_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DeleteTagFromObjsOption_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AddTagToSelectedObjsOption_Click(object sender, EventArgs e)
        {
            string tag = ((sender as ToolStripMenuItem)?.Owner as ContextMenuStrip)?.SourceControl?.Text;
            if(tag == null || _selectedObjects.Count == 0) { return; }

            throw new NotImplementedException();
        }
        #endregion
    }
}
