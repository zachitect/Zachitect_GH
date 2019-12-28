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
    public class ResetDefaultTheme : GH_Component
    {

        public ResetDefaultTheme()
          : base(
                "Default Theme",//Full Name
                "Reset Theme",//Nick Name
                "Reset default theme for Grasshopper.\n\n© Zach X.G. Zheng | Zachitect.com",//Description
                "Zachitect.com",//Tab
                "Theme"//Category
                )
        {
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Reset Default Theme", "Reset", "Toggle to reset to default theme for Grasshopper", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool Activate = new bool();
            if (!DA.GetData(0, ref Activate)) return;

            if (Activate == true)
            {
                gs.canvas_mono = false;
                gs.canvas_shade_size = 30;
                gs.canvas_grid_col = 150;
                gs.canvas_grid_row = 50;

                int[] default_raws = new int[9]
                {
                -1,
                -2830136,
                -16777216,
                1342177280,
                503316480,
                -1778384896,
                -1258341376,
                -8531416,
                838860800
                };

                gs.canvas_mono_color = System.Drawing.Color.FromArgb(-1);
                gs.canvas_back = System.Drawing.Color.FromArgb(-2830136);
                gs.canvas_edge = System.Drawing.Color.FromArgb(-16777216);
                gs.canvas_shade = System.Drawing.Color.FromArgb(1342177280);
                gs.canvas_grid = System.Drawing.Color.FromArgb(503316480);
                gs.wire_default = System.Drawing.Color.FromArgb(-1778384896);
                gs.wire_empty = System.Drawing.Color.FromArgb(-1258341376);
                gs.wire_selected_a = System.Drawing.Color.FromArgb(-8531416);
                gs.wire_selected_b = System.Drawing.Color.FromArgb(838860800);
            }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.ResetDefaultTheme;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("4e56c4fc-f3f6-41e0-9b63-105381870f97"); }
        }
    }
}
