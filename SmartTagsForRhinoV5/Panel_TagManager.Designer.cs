using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SmartTagsForRhino
{
    public enum TagButtonState { ACTIVE, INACTIVE }

    public partial class Panel_TagManager
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.SuspendLayout();
            // 
            // pnlTagContainer
            // 
            this.pnlTagContainer.Location = new System.Drawing.Point(0, 34);
            this.pnlTagContainer.Name = "pnlTagContainer";
            this.pnlTagContainer.Size = new System.Drawing.Size(326, 584);
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
            // Panel_TagManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlTagContainer);
            this.Controls.Add(this.pnlTitleBar);
            this.Name = "Panel_TagManager";
            this.Size = new System.Drawing.Size(326, 618);
            this.ResumeLayout(false);
        }

        private Panel pnlTitleBar;
        private FlowLayoutPanel pnlTagContainer;
        #endregion

        #region properties
        #endregion

        #region methods
        private void AddNewTagButton(string tagName, TagButtonState state)
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

            btn.TabIndex = _tagDict.Count + 1;
            btn.Text = tagName;
            btn.UseVisualStyleBackColor = false;

            StyleButton(ref btn, state);

            this.pnlTagContainer.Controls.Add(btn);
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
            AddNewTagButton(tagBtn.TagName, tagBtn.State);
        }

        private void ResetUI()
        {
            //clear everything and repopulate the UI
            this.pnlTagContainer.Controls.Clear();
            var tags = _tagDict.Keys.ToList();
            tags.Sort();
            foreach(var key in tags)
            {
                AddNewTagButton(_tagDict[key]);
            }
        }

        private void UpdateUI()
        {
            foreach(var control in this.pnlTagContainer.Controls)
            {
                if(!(control is Button)) { continue; }
                var btn = (Button)control;
                TagButton tagBtn;
                if(!_tagDict.TryGetValue(btn.Text, out tagBtn)) { continue; }
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
        #endregion
    }
}
