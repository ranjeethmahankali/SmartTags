using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System.Text;
using Rhino.DocObjects;
using System.Linq;
using System.Diagnostics;

namespace SmartTagsForRhino.Commands
{
    //this command is the one that adds new tags to objects
    [System.Runtime.InteropServices.Guid("e6de5dc7-d49e-40a5-a445-38e6b791c95e")]
    public class SaveCurrentFilter : Command
    {
        public static string CommandString = "SaveCurrentTagFilter";
        public SaveCurrentFilter()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static SaveCurrentFilter Instance
        {
            get; private set;
        }

        public override string EnglishName
        {
            get { return CommandString; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            if (Panel_TagManager.CurrentFilter == null)
            {
#if RhinoV5
                Rhino.UI.Dialogs.ShowMessageBox("Current filter is not set.", "Cannot Save Filter");
#elif RhinoV6
                Rhino.UI.Dialogs.ShowMessage("Current filter is not set.", "Cannot Save Filter");
#endif
                return Result.Nothing;
            }

            string filterName;
            using (GetString getter = new GetString())
            {
                getter.AcceptString(true);
                getter.SetCommandPrompt("Enter a name for the current filter");
                if (getter.Get() != GetResult.String)
                {
                    RhinoApp.WriteLine("Invalid Input");
                    return getter.CommandResult();
                }
                filterName = getter.StringResult();
            }

            string filterText = Panel_TagManager.CurrentFilter.ToString();
            if (Panel_TagManager.SavedFilters.ContainsKey(filterName))
            {
                Panel_TagManager.SavedFilters[filterName] = filterText;
            }
            else
            {
                Panel_TagManager.SavedFilters.Add(filterName, filterText);
            }

            TagUtil.AddSavedFiltersToDocument(doc);
            doc.Modified = true;
            return Result.Success;
        }
    }
}
