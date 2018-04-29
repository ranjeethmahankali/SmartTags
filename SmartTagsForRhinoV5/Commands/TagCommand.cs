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
    [System.Runtime.InteropServices.Guid("137386c0-bfda-4376-9223-f9452fc9dec4")]
    public class TagCommand : Command
    {
        public TagCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }
        ///<summary>The only instance of this command.</summary>
        public static TagCommand Instance
        {
            get; private set;
        }
        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "Tag"; }
        }
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            List<RhinoObject> objs = new List<RhinoObject>();
            using (GetObject getter = new GetObject())
            {
                getter.EnablePreSelect(true, false);
                getter.SetCommandPrompt("Select all the objects you want to tag");
                getter.AcceptColor(false);
                getter.AcceptString(false);
                getter.GroupSelect = true;
                getter.GetMultiple(1, 0);
                if (getter.CommandResult() != Result.Success) { return getter.CommandResult(); }

                for (int i = 0; i < getter.ObjectCount; i++)
                {
                    objs.Add(getter.Object(i).Object());
                }
            }

            string tagName;
            using (GetString getter = new GetString())
            {
                getter.SetCommandPrompt("Enter the tag");
                if (getter.Get() != GetResult.String)
                {
                    RhinoApp.WriteLine("Invalid Input for tag");
                    return getter.CommandResult();
                }
                tagName = getter.StringResult();
            }

            TagUtil.AddTag(objs, tagName, true);

            doc.Views.Redraw();
            return Result.Success;
        }
    }
}