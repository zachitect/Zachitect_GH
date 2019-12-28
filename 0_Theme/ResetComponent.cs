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
    public class ResetComponent : GH_Component
    {

        public ResetComponent()
          : base(
                "Reset Component",//Full Name
                "Reset Node",//Nick Name
                "Reset component theme to default.\n\n© Zach X.G. Zheng | Zachitect.com",//Description
                "Zachitect.com",//Tab
                "Theme"//Category
                )
        {
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Reset Component Theme", "Reset", "Reset component theme to default", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool ResetCompTheme = false;
            if (!DA.GetData(0, ref ResetCompTheme)) return;

            if (ResetCompTheme == true)
            {
                System.Drawing.Color Black = System.Drawing.Color.FromArgb(255, 0, 0, 0);
                System.Drawing.Color StanNorm = System.Drawing.Color.FromArgb(255, 199, 199, 199);
                System.Drawing.Color StanSele = System.Drawing.Color.FromArgb(255, 129, 214, 49);
                System.Drawing.Color HiddNorm = System.Drawing.Color.FromArgb(255, 140, 140, 155);
                System.Drawing.Color HiddSele = System.Drawing.Color.FromArgb(255, 80, 180, 10);

                gs.palette_normal_standard = new gg.Canvas.GH_PaletteStyle(StanNorm, Black, Black);
                gs.palette_normal_selected = new gg.Canvas.GH_PaletteStyle(StanSele, Black, Black);
                gs.palette_hidden_standard = new gg.Canvas.GH_PaletteStyle(HiddNorm, Black, Black);
                gs.palette_hidden_selected = new gg.Canvas.GH_PaletteStyle(HiddSele, Black, Black);
            }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.ResetComponent;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("d2438675-5594-401c-8818-9fa70648782d"); }
        }
    }
}
