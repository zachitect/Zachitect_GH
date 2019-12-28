using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Display;
using Rhino.Geometry;

namespace Zachitect_GH
{
    public class LinePreview : GH_Component
    {

        public LinePreview()
          : base(
                "Curve Preview",//Full Name
                "C Preview",//Nick Name
                "Custom preview of curves\n\n© Zach X.G. Zheng | Zachitect.com",//Description
                "Zachitect.com",//Tab
                "Annotation"//Category
                )
        {
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Preview Curve", "Curve", "Curve to be displayed in preview", GH_ParamAccess.item);
            pManager.AddColourParameter("Preview Colour", "Colour", "Colour of the curve in preview", GH_ParamAccess.item, System.Drawing.Color.FromArgb(255,100,0,17));
            pManager.AddIntegerParameter("Line Weight", "Thickness", "Thickness of the curve in integer", GH_ParamAccess.item, 1);
            pManager.AddBooleanParameter("Screen Scale", "Screen", "Toggle on to display curve by screen scale", GH_ParamAccess.item, false);
            pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }
        protected override void BeforeSolveInstance()
        {
            _clippingBox = BoundingBox.Empty;
            _curves.Clear();
            _colours.Clear();
            _widths.Clear();
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve PreCrv = null;
            System.Drawing.Color PreCol = new System.Drawing.Color();
            int LineWeight = new int();
            bool ScreenBool = false;

            if (!DA.GetData(0, ref PreCrv)) return;
            if (!DA.GetData(1, ref PreCol)) return;
            if (!DA.GetData(2, ref LineWeight)) return;
            DA.GetData(3, ref ScreenBool);
            RelaScale = ScreenBool;

            _clippingBox = BoundingBox.Union(_clippingBox, PreCrv.GetBoundingBox(false));

            _curves.Add(PreCrv);
            _colours.Add(PreCol);
            _widths.Add(LineWeight);
        }
        private bool RelaScale = false;
        private BoundingBox _clippingBox;
        private readonly List<Curve> _curves = new List<Curve>();
        private readonly List<System.Drawing.Color> _colours = new List<System.Drawing.Color>();
        private readonly List<int> _widths = new List<int>();


        public override BoundingBox ClippingBox
        {
            get { return _clippingBox; }
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            if(_curves.Count > 0 && _colours.Count == _curves.Count && _widths.Count == _curves.Count)
            {
                args.Viewport.GetWorldToScreenScale(_clippingBox.Center, out Double PixelPerUnit);
                for (int i = 0; i < _curves.Count; i++)
                {
                    if (RelaScale == true)
                    {
                        args.Display.DrawCurve(_curves[i], _colours[i], _widths[i]);
                    }
                    else
                    {
                        Double WeightTo = Math.Ceiling(_widths[i] * PixelPerUnit);
                        int Weight = Convert.ToInt32(WeightTo);
                        args.Display.DrawCurve(_curves[i], _colours[i], Weight);
                    }   
                }
            }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.LinePreview;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("0a891720-6474-4214-9aaa-ced6fb2b2aa3"); }
        }
    }
}
