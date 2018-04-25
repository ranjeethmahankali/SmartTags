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

namespace SmartTags
{
    //this command is the one that adds new tags to objects
    [System.Runtime.InteropServices.Guid("137386c0-bfda-4376-9223-f9452fc9dec4")]
    public class SmartTagsCommand : Command
    {
        public SmartTagsCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }
        ///<summary>The only instance of this command.</summary>
        public static SmartTagsCommand Instance
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
                //Debug.WriteLine(tagName, "Tag");
            }

            foreach (RhinoObject obj in objs)
            {
                SmartTag.AddTag(obj, tagName);
            }

            doc.Views.Redraw();
            return Result.Success;
        }
    }

    //this command deletes tags associated with the selected objects
    [System.Runtime.InteropServices.Guid("63083f7b-f7b0-4479-9a64-a4403d66e25c")]
    public class DeleteTagsCommand : Command
    {
        public DeleteTagsCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }
        ///<summary>The only instance of this command.</summary>
        public static DeleteTagsCommand Instance
        {
            get; private set;
        }
        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "DeleteTag"; }
        }
        //this is where the logic of the command is defined
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            List<RhinoObject> objs = new List<RhinoObject>();
            using (GetObject getter = new GetObject())
            {
                getter.EnablePreSelect(true, false);
                getter.SetCommandPrompt("Select all the objects you want to tag");
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
                //Debug.WriteLine(tagName, "Tag");
            }

            bool success = true;
            foreach (RhinoObject obj in objs)
            {
                success = success && SmartTag.DeleteTag(obj, tagName);
            }
            return success ? Result.Success : Result.Nothing;
        }
    }

    //this commands accepts a tag filter from user as a string and selects the elements
    //that pass the filter.
    [System.Runtime.InteropServices.Guid("a5531951-5e9a-4797-bc20-63511752227e")]
    public class TagFilterCommand : Command
    {
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
            get { return "TagFilter"; }
        }
        //this is where the logic of the command is defined
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            string filterStr;
            Rhino.UI.Dialogs.ShowEditBox(
                "Tag Filter",
                "Enter the boolean filter statement (use the keywords 'and', 'or'.)",
                "",
                false,
                out filterStr
                );

            Stopwatch watch = new Stopwatch();
            watch.Start();
            List<Guid> filtered = SmartTag.Evaluate(filterStr, ref doc);
            Debug.WriteLine(watch.ElapsedMilliseconds, "Time");
            watch.Stop();

            doc.Objects.UnselectAll();
            doc.Objects.Select(filtered);

            return Result.Success;
        }
    }

    //this displays a dialog showing all the tags associated with the selected objects
    [System.Runtime.InteropServices.Guid("43f54dba-dd24-459a-924d-fe369ee30bdb")]
    public class ShowTagsCommand : Command
    {
        public ShowTagsCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }
        ///<summary>The only instance of this command.</summary>
        public static ShowTagsCommand Instance
        {
            get; private set;
        }
        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "ShowTags"; }
        }
        //this is where the logic of the command is defined
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            List<RhinoObject> objs = new List<RhinoObject>();
            using (GetObject getter = new GetObject())
            {
                getter.EnablePreSelect(true, false);
                getter.SetCommandPrompt("Select all the objects you want to tag");
                getter.GroupSelect = true;
                getter.GetMultiple(1, 0);
                if (getter.CommandResult() != Result.Success) { return getter.CommandResult(); }

                for (int i = 0; i < getter.ObjectCount; i++)
                {
                    objs.Add(getter.Object(i).Object());
                }
            }

            List<string> tags = new List<string>();
            foreach (RhinoObject obj in objs)
            {
                tags = tags.Union(SmartTag.GetTags(obj)).ToList();
            }

            StringBuilder str = new StringBuilder();
            foreach (string tag in tags)
            {
                str.AppendLine(tag);
            }

            Rhino.UI.Dialogs.ShowTextDialog(str.ToString(), "Tags in this document");
            //now process what is in the filterStr and do stuff
            return Result.Success;
        }
    }
}