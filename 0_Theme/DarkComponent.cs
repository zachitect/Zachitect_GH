using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using gs = Grasshopper.GUI.Canvas.GH_Skin;
using gg = Grasshopper.GUI;

namespace Zachitect_GH
{
    public class DarkComponent : GH_Component
    {

        public DarkComponent()
          : base(
                "Dark Component",//Full Name
                "Dark Node",//Nick Name
                "Activate dark theme for Grasshopper components.\n\n© Zach X.G. Zheng | Zachitect.com",//Description
                "Zachitect.com",//Tab
                "Theme"//Category
                )
        {
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Activate Dark Component", "Activate", "Activate dark theme for Grasshopper component", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool DarkComponent = false;
            if (!DA.GetData(0, ref DarkComponent)) return;

            if (DarkComponent == true)
            {
                System.Drawing.Color Blue = System.Drawing.Color.FromArgb(255, 0, 127, 159);
                System.Drawing.Color Green = System.Drawing.Color.FromArgb(255, 70, 150, 50);
                System.Drawing.Color White = System.Drawing.Color.FromArgb(255, 255, 255, 255);

                gs.palette_normal_standard = new gg.Canvas.GH_PaletteStyle(Blue, Blue, White);
                gs.palette_normal_selected = new gg.Canvas.GH_PaletteStyle(Green, Green, White);
                gs.palette_hidden_standard = gs.palette_black_standard;
            }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.DarkComponent;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("f6a0d1f6-f8b4-4b43-a72a-efd3cafbd409"); }
        }
    }
}
