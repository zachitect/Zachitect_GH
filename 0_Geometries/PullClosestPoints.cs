using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Zachitect_GH
{
    public class PullClosestPoints : GH_Component
    {

        public PullClosestPoints()
          : base(
                "Pull Closest Points",//Full Name
                "Pull Points",//Nick Name
                "Pull closest points to curves and return points on curve, curve parameters at points, distance, and curve index.\n\n© Zach X.G. Zheng | Zachitect.com",//Description
                "Zachitect.com",//Tab
                "Geometries"//Category
                )
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "Points", "Points to be pulled to curves", GH_ParamAccess.list);
            pManager.AddCurveParameter("Curves", "Curves", "Curves to pull points onto", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Closest Points", "Closest", "Closest points on curve to input points", GH_ParamAccess.list);
            pManager.AddNumberParameter("Curve Parameters", "Parameters", "Parameters of curves at closest points", GH_ParamAccess.list);
            pManager.AddNumberParameter("Shortest Distancse", "Distances", "Shortest distances between input points and curves", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Curve Index", "Index", "Index of closest curves to input points", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> InPoints = new List<Point3d>();
            if (!DA.GetDataList(0, InPoints)) return;
            List<Curve> InCurves = new List<Curve>();
            if (!DA.GetDataList(1, InCurves)) return;

            RunPullPoints(InPoints, InCurves);
            DA.SetDataList(0, CPTS);
            DA.SetDataList(1, PARA);
            DA.SetDataList(2, DIST);
            DA.SetDataList(3, CIND);
        }

        Point3d[] CPTS { get; set; }
        Double[] PARA { get; set; }
        Double[] DIST { get; set; }
        int[] CIND { get; set; }

        private void RunPullPoints(List<Point3d> LPts, List<Curve> LCrvs)
        {
            Point3d[] ClosestPts = new Point3d[LPts.Count];
            Double[] Parameters = new double[LPts.Count];
            Double[] Distances = new double[LPts.Count];
            int[] CurveIndex = new int[LPts.Count];

            for (int i = 0; i < LPts.Count; i ++)
            {
                Point3d Pt = new Point3d();
                Double Param = -1;
                Double Dist = double.PositiveInfinity;
                int Index = -1;

                for (int j = 0; j < LCrvs.Count; j++)
                {
                    Double param;
                    LCrvs[j].ClosestPoint(LPts[i], out param);
                    Point3d clpt = LCrvs[j].PointAt(param);
                    Double dist = LPts[i].DistanceTo(clpt);
                    if(dist <= Dist)
                    {
                        Dist = dist;
                        Param = param;
                        Pt = clpt;
                        Index = j;
                    }
                }
                ClosestPts[i] = Pt;
                Parameters[i] = Param;
                Distances[i] = Dist;
                CurveIndex[i] = Index;
            }
            CPTS = ClosestPts;
            PARA = Parameters;
            DIST = Distances;
            CIND = CurveIndex;
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.PullClosestPoints;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("b69a34fc-f347-4c65-9bc2-437e22c908fe"); }
        }
    }
}
