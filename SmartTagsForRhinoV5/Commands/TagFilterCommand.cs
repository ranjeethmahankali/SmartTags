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
    //this commands accepts a tag filter from user as a string and selects the elements
    //that pass the filter.
    [System.Runtime.InteropServices.Guid("a5531951-5e9a-4797-bc20-63511752227e")]
    public class TagFilterCommand : Command
    {
        public static string CommandString = "TagFilter";
        public TagFilterCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }
        ///<summary>The only instance of this command.</summary>
        public static TagFilterCommand Instance
        {
            get; private set;
        }
        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return CommandString; }
        }
        //this is where the logic of the command is defined
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            string filterStr;
            using (GetString getter = new GetString())
            {
                getter.AcceptString(true);
                getter.SetCommandPrompt("Enter the boolean filter statement (in quotes)");
                if (getter.Get() != GetResult.String)
                {
                    RhinoApp.WriteLine("Invalid Input for tag");
                    return getter.CommandResult();
                }
                filterStr = getter.StringResult();
            }

            Stopwatch watch = new Stopwatch();
            watch.Start();
            List<Guid> filtered = TagUtil.Evaluate(filterStr, ref doc);
            Debug.WriteLine(watch.ElapsedMilliseconds, "Time");
            watch.Stop();

            doc.Objects.UnselectAll();
            doc.Objects.Select(filtered);
            doc.Views.Redraw();

            return Result.Success;
        }
    }
}