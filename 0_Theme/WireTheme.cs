using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using gs = Grasshopper.GUI.Canvas.GH_Skin;
using gu = Grasshopper.GUI.GH_GraphicsUtil;

namespace Zachitect_GH
{
    public class WireTheme : GH_Component
    {

        public WireTheme()
          : base(
                "Wire Theme",//Full Name
                "Wire Theme",//Nick Name
                "Customising theme of Grasshopper component wires.\n\n© Zach X.G. Zheng | Zachitect.com",//Description
                "Zachitect.com",//Tab
                "Theme"//Category
                )
        {
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddColourParameter("Normal Wire", "Normal Wire", "Set colour of normal wire", GH_ParamAccess.item);
            pManager.AddColourParameter("Empty Wire", "Empty Wire", "Set colour of empty wireS", GH_ParamAccess.item);
            pManager.AddColourParameter("Wire Start", "Wire Start", "Set colour of wire start", GH_ParamAccess.item);
            pManager.AddColourParameter("Wire End", "Wire End", "Set colour of wire end", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            System.Drawing.Color Wire_Normal = gs.wire_default;
            System.Drawing.Color Wire_Empty = gs.wire_empty;
            System.Drawing.Color Wire_Start = gs.wire_selected_a;
            System.Drawing.Color Wire_End = gs.wire_selected_b;

            DA.GetData(0, ref Wire_Normal);
            DA.GetData(1, ref Wire_Empty);
            DA.GetData(2, ref Wire_Start);
            DA.GetData(3, ref Wire_End);

            gs.wire_default = Wire_Normal;
            gs.wire_empty = Wire_Empty;
            gs.wire_selected_a = Wire_Start;
            gs.wire_selected_b = Wire_End;
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.WireTheme;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("12253020-c060-42ce-8073-91caa6c17cdd"); }
        }
    }
}
