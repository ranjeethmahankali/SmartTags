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
    //this command deletes tags associated with the selected objects
    [System.Runtime.InteropServices.Guid("63083f7b-f7b0-4479-9a64-a4403d66e25c")]
    public class TagsFromLayers : Command
    {
        public static string CommandString = "DeleteTag";
        public TagsFromLayers()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }
        ///<summary>The only instance of this command.</summary>
        public static TagsFromLayers Instance
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
            foreach (var layer in doc.Layers)
            {
                RhinoObject[] objs = doc.Objects.FindByLayer(layer);
                TagUtil.AddTag(objs, layer.Name, true);
            }

            doc.Views.Redraw();
            return Result.Success;
        }
    }
}