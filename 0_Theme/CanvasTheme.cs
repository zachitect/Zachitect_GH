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
    public class CanvasTheme : GH_Component
    {

        public CanvasTheme()
          : base(
                "Canvas Theme",//Full Name
                "Canvas Theme",//Nick Name
                "Customising theme of Grasshopper canvas.\n\n© Zach X.G. Zheng | Zachitect.com",//Description
                "Zachitect.com",//Tab
                "Theme"//Category
                )
        {
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Monochrome", "Monochrome", "Toggle monochrome background", GH_ParamAccess.item, false);
            pManager.AddColourParameter("Canvas Colour", "Canvas", "Set canvas background colour", GH_ParamAccess.item);
            pManager.AddColourParameter("Edge Colour", "Edge", "Set canvas edge colour", GH_ParamAccess.item);
            pManager.AddColourParameter("Shade Colour", "Shade", "Set canvas shade colour", GH_ParamAccess.item);
            pManager.AddColourParameter("Grid Colour", "Grid", "Set canvas grid colour", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool Monochrome = new bool();
            System.Drawing.Color Canvas = gs.canvas_back;
            System.Drawing.Color Edge = gs.canvas_edge;
            System.Drawing.Color Shade = gs.canvas_back;
            System.Drawing.Color Grid = gs.canvas_grid;
            DA.GetData(0, ref Monochrome);
            DA.GetData(1, ref Canvas);
            DA.GetData(2, ref Edge);
            DA.GetData(3, ref Shade);
            DA.GetData(4, ref Grid);

            if (Monochrome == true)
            {
                gs.canvas_mono = Monochrome;
            }
            gs.canvas_shade = Shade;
            gs.canvas_back = Canvas;
            gs.canvas_mono_color = Canvas;
            gs.canvas_edge = Edge;
            gs.canvas_grid = Grid;
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.CanvasTheme;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("9f11547a-7c8b-4ba5-879c-a371821b0816"); }
        }
    }
}
