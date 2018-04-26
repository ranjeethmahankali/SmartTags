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
    [System.Runtime.InteropServices.Guid("980edb5d-dc34-4118-9f46-a35426110be8")]
    public class TagManagerCommand : Command
    {
        public TagManagerCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static TagManagerCommand Instance
        {
            get; private set;
        }

        public override string EnglishName
        {
            get { return "TagManager"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var type = typeof(Panel_TagManager);
            Rhino.UI.Panels.OpenPanel(type.GUID);
            return Result.Success;
        }
    }

    //this command is the one that adds new tags to objects
    [System.Runtime.InteropServices.Guid("ed10dab2-9276-4c5f-87f0-48bbe149e49d")]
    public class CloseTagManagerCommand : Command
    {
        public CloseTagManagerCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static CloseTagManagerCommand Instance
        {
            get; private set;
        }

        public override string EnglishName
        {
            get { return "CloseTagManager"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var type = typeof(Panel_TagManager);
            Rhino.UI.Panels.ClosePanel(type.GUID);
            return Result.Success;
        }
    }
}
