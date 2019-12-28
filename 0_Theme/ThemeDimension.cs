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
    public class ThemeDimension : GH_Component
    {

        public ThemeDimension()
          : base(
                "Theme Dimension",//Full Name
                "Theme Dimension",//Nick Name
                "Adjusting canvas grid shade and dimension.\n\n© Zach X.G. Zheng | Zachitect.com",//Description
                "Zachitect.com",//Tab
                "Theme"//Category
                )
        {
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Shade Size", "Shade Size", "Adjust canvas shade size", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Grid Width", "Grid Width", "Adjust grid width", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Grid Height", "Grid Height", "Adjust grid height", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int ShadeSize = gs.canvas_shade_size;
            int ColumnSize = gs.canvas_grid_col;
            int RowSize = gs.canvas_grid_row;

            DA.GetData(0, ref ShadeSize);
            DA.GetData(1, ref ColumnSize);
            DA.GetData(2, ref RowSize);

            gs.canvas_shade_size = ShadeSize;
            gs.canvas_grid_col = ColumnSize;
            gs.canvas_grid_row = RowSize;
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.ThemeDimension;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("50b8be4d-081c-4328-8d8e-a836c98f8753"); }
        }
    }
}
