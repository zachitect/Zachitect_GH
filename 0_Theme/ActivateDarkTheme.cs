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
    public class ActivateDarkTheme : GH_Component
    {

        public ActivateDarkTheme()
          : base(
                "Dark Theme",//Full Name
                "Dark Theme",//Nick Name
                "Activate dark theme for Grasshopper.\n\n© Zach X.G. Zheng | Zachitect.com",//Description
                "Zachitect.com",//Tab
                "Theme"//Category
                )
        {
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Activate Dark Theme", "Activate", "Toggle to activate dark theme for Grasshopper", GH_ParamAccess.item, false);
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
                gs.canvas_mono_color = gu.ColourARGB(42, 42, 42);
                gs.canvas_shade = gu.ColourARGB(0, 0, 0, 0);
                gs.canvas_back = gu.ColourARGB(42, 42, 42);
                gs.canvas_edge = gu.ColourARGB(53, 53, 53);
                gs.canvas_grid = gu.ColourARGB(53, 53, 53);
                gs.canvas_shade_size = 0;
                gs.canvas_grid_col = 150;
                gs.canvas_grid_row = 150;
                gs.wire_default = gu.ColourARGB(0, 127, 159);
                gs.wire_empty = gu.ColourARGB(255, 150, 75);
                gs.wire_selected_a = gu.ColourARGB(70, 150, 40);
                gs.wire_selected_b = gu.ColourARGB(70, 150, 40);
            }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.ActivateDarkTheme;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("3115ab40-4ad4-4c97-93a2-28b8fe37a79e"); }
        }
    }
}
