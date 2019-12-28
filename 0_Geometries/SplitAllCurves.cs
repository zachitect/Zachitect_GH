using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Zachitect_GH
{
    public class SplitAllCurves : GH_Component
    {

        public SplitAllCurves()
          : base(
                "Split All Curves",//Full Name
                "X Curves",//Nick Name
                "Split all curves with one another including self-intersected curves.\n\n© Zach X.G. Zheng | Zachitect.com",//Description
                "Zachitect.com",//Tab
                "Geometries"//Category
                )
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "Curve", "Curve(s) to split one another or itself", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Split Curves", "Split Curves", "All curves split by one another", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> InputCrvs = new List<Curve>();
            if (!DA.GetDataList(0, InputCrvs)) return;
            Grasshopper.Kernel.Data.GH_Structure<GH_Curve> OutputCurves = MultiCurveSplit(InputCrvs.ToArray());
            DA.SetDataTree(0, OutputCurves);
        }
        Double MTolerance = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
        private Grasshopper.Kernel.Data.GH_Structure<GH_Curve> MultiCurveSplit(Curve[] CurveArr)
        {
            List<Double>[] CurveParametersArr = new List<double>[CurveArr.Length];
            for (int i = 0; i < CurveArr.Length; i++)
            {
                List<Double> CurveParameters = new List<double>();

                Rhino.Geometry.Intersect.CurveIntersections SelfInter = Rhino.Geometry.Intersect.Intersection.CurveSelf(CurveArr[i], MTolerance);
                for (int a = 0; a < SelfInter.Count; a++)
                {
                    CurveParameters.Add(SelfInter[a].ParameterA);
                    CurveParameters.Add(SelfInter[a].ParameterB);
                }


                for (int j = 0; j < CurveArr.Length; j++)
                {
                    Rhino.Geometry.Intersect.CurveIntersections CurveInter = Rhino.Geometry.Intersect.Intersection.CurveCurve(CurveArr[i], CurveArr[j], MTolerance, MTolerance);
                    for (int k = 0; k < CurveInter.Count; k++)
                    {
                        CurveParameters.Add(CurveInter[k].ParameterA);
                    }
                }
                HashSet<Double> ParametersSet = new System.Collections.Generic.HashSet<Double>(CurveParameters);
                CurveParametersArr[i] = ParametersSet.ToList();
            }
            Grasshopper.Kernel.Data.GH_Structure<GH_Curve> outTree = new Grasshopper.Kernel.Data.GH_Structure<GH_Curve>();
            for (int i = 0; i < CurveArr.Length; i++)
            {
                Grasshopper.Kernel.Data.GH_Path path = new Grasshopper.Kernel.Data.GH_Path(i);
                Curve[] CurveSplit = CurveArr[i].Split(CurveParametersArr[i]);
                foreach (Curve cs in CurveSplit)
                {
                    GH_Curve cc = new GH_Curve(cs);
                    outTree.Append(cc, path);
                }
            }
            return outTree;
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.SplitAllCurves;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("efc761ed-39b2-495e-9786-280a3f42046d"); }
        }
    }
}
