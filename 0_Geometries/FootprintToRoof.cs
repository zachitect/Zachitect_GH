using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Zachitect_GH
{
    public class FootprintToRoof : GH_Component
    {

        public FootprintToRoof()
          : base(
                "Footprint Roof",//Full Name
                "Roof",//Nick Name
                "Generate a hip roof based on input footprint.\n\n© Zach X.G. Zheng | Zachitect.com",//Description
                "Zachitect.com",//Tab
                "Geometries"//Category
                )
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Roof Footprint", "Roof Footprint", "Closed polyline curve as roof footprint", GH_ParamAccess.item);
            pManager.AddNumberParameter("Slope Angle", "Slope Angle", "Default = 20, average angle of roof slope in degree", GH_ParamAccess.item, 20);
            pManager.AddNumberParameter("Myth Factor", "Myth Factor", "Default = 0.2, roof face extent factor, efer to output parameter - trimmed faces", GH_ParamAccess.item, 0.2);
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Roof Edges", "Roof Edges", "Unified clockwise roof edges", GH_ParamAccess.list);
            pManager.AddVectorParameter("Slope Vectors", "Slope Vectors", "Vector of each slope", GH_ParamAccess.list);
            pManager.AddVectorParameter("Hip Vectors", "Hip Vectors", "Vector of each hip", GH_ParamAccess.list);
            pManager.AddBrepParameter("Face Breps", "Face Breps", "Untrimmed Roof Faces", GH_ParamAccess.list);
            pManager.AddBrepParameter("Trimmed Faces", "Trimmed Faces", "Trimmed Roof Faces with Co-planar ones merged", GH_ParamAccess.list);
            pManager.AddBrepParameter("Roof Solid", "Roof Solid", "Hip roof geometry from input footprint", GH_ParamAccess.item);
        }

        //Common Settings
        Double MTolerance = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
        int EdgeCount { get; set; }
        double SlopeRad { get; set; }
        double LineLength { get; set; }

        private Vector3d[] GetSlopeVector(Line[] Edges, int Count, double SlopeRadian)
        {
            Vector3d[] VecRoofSlopes = new Vector3d[Count];
            for (int i = 0; i < Count; i++)
            {
                Vector3d EdgeNormal = Vector3d.CrossProduct(Edges[i].Direction, Rhino.Geometry.Vector3d.ZAxis);
                bool RotateEdgeNormal = EdgeNormal.Rotate(SlopeRadian, Edges[i].Direction);
                if (RotateEdgeNormal == false)
                {
                    return null;
                }
                if (EdgeNormal.Unitize() == false)
                {
                    return null;
                }
                VecRoofSlopes[i] = EdgeNormal;
            }
            return VecRoofSlopes;
        }
        private Vector3d[] GetHipVector(Line[] Edges, Vector3d[] SlopeVectors, int Count)
        {
            Vector3d[] HipVectors = new Vector3d[Count];
            for (int i = 0; i < Count; i++)
            {
                //Set Rotate Index
                int prev = i - 1;
                if (prev < 0)
                {
                    prev = prev + Count;
                }
                int next = i + 1;
                if (next >= Count)
                {
                    next = next - Count;
                }

                //Get Projected Slope Vector ABS
                double SlopeVecABS = new Vector3d(SlopeVectors[prev].X, SlopeVectors[prev].Y, Edges[prev].FromZ).Length;
                Vector3d VecEA = new Vector3d(Edges[prev].From - Edges[prev].To);
                Vector3d VecEB = new Vector3d(Edges[i].To - Edges[i].From);
                VecEA.Unitize(); VecEB.Unitize();

                double HalfRadian = Vector3d.VectorAngle(VecEA, VecEB, Rhino.Geometry.Vector3d.ZAxis) / 2;
                double HipProjectVecABS = SlopeVecABS / Math.Sin(HalfRadian);

                Vector3d HipProjectVec = (VecEA + VecEB) / 2;
                HipProjectVec.Unitize();

                if (HalfRadian > Math.PI / 2)
                {
                    HipProjectVec.Reverse();
                }

                Vector3d HipProjectVecScaled = HipProjectVec * HipProjectVecABS;
                Vector3d HipVector = HipProjectVecScaled + new Vector3d(0, 0, SlopeVectors[prev].Z);
                HipVector.Unitize();
                HipVectors[i] = HipVector;
            }
            return HipVectors;
        }

        private Brep[] GetUntrimmedRoofBreps(Line[] EdgeLines, Vector3d[] HipVectors, int Count, double LLength)
        {
            Brep[] UntrimmedFaces = new Brep[Count];
            for (int i = 0; i < Count; i++)
            {
                //Set Rotate Index
                int prev = i - 1;
                if (prev < 0)
                {
                    prev = prev + Count;
                }
                int next = i + 1;
                if (next >= Count)
                {
                    next = next - Count;
                }

                //Calculation
                Vector3d VecA = new Vector3d(HipVectors[i]);
                Vector3d VecB = new Vector3d(HipVectors[next]);
                Line A = new Line(EdgeLines[i].From, VecA, LLength);
                Line B = new Line(EdgeLines[i].To, VecB, LLength);

                bool Intersection = Rhino.Geometry.Intersect.Intersection.LineLine(A, B, out double pa, out double pb, MTolerance, true);
                if(Intersection == true)
                {
                    UntrimmedFaces[i] = Brep.CreateFromCornerPoints(A.From, A.PointAt(pa), B.PointAt(pb), B.From, MTolerance);
                }
                else
                {
                    UntrimmedFaces[i] = Brep.CreateFromCornerPoints(A.From, A.To, B.To, B.From, MTolerance);
                }
            }
            return UntrimmedFaces;
        }
        private Point3d[] BrepVertexToPtsPlus(Brep BR)
        {
            List<Point3d> PList = BR.DuplicateVertices().ToList();
            PList.Add(BR.DuplicateVertices()[0]);
            return PList.ToArray();
        }
        private Brep[] CleanedUntrimmedBreps(Brep[] UntrimmedBreps, int Count)
        {
            List<Brep> CleanBreps = new List<Brep>();
            for (int i = 0; i < Count; i++)
            {
                if(UntrimmedBreps[i] != null)
                {
                    Point3d[] VI = BrepVertexToPtsPlus(UntrimmedBreps[i]);
                    List<PolylineCurve> PC = new List<PolylineCurve>();
                    List<int> Index = new List<int>();
                    for (int j = 0; j < Count; j++)
                    {
                        if (UntrimmedBreps[j] != UntrimmedBreps[i] && UntrimmedBreps[i] != null && UntrimmedBreps[j] != null)
                        {
                            Point3d[] VJ = BrepVertexToPtsPlus(UntrimmedBreps[j]);
                            Point3d[] VX = new Point3d[VI.Length + VJ.Length];
                            Array.Copy(VI, VX, VI.Length);
                            Array.Copy(VJ, 0, VX, VI.Length, VJ.Length);
                            bool PlanarBool = Point3d.ArePointsCoplanar(VX, MTolerance);
                            if (PlanarBool == true)
                            {
                                Index.Add(j);
                                PC.Add(new PolylineCurve(VJ));
                            }
                        }
                    }
                    if (Index.Count > 0)
                    {
                        Index.Add(i);
                        PC.Add(new PolylineCurve(VI));
                        Curve[] UC = PolylineCurve.CreateBooleanUnion(PC, MTolerance);
                        foreach (Curve uc in UC)
                        {
                            foreach (Brep b in Brep.CreatePlanarBreps(uc, MTolerance))
                            {
                                CleanBreps.Add(b);
                            }
                        }
                        foreach (int index in Index)
                        {
                            UntrimmedBreps[index] = null;
                        }
                    }
                }
            }
            foreach(Brep b in UntrimmedBreps)
            {
                if(b != null)
                {
                    CleanBreps.Add(b);
                }
            }
            return CleanBreps.ToArray();
        }
        private Brep[] GetBrepTrimmed(Brep[] MergedBreps, PolylineCurve FootprintCurve, int Count)
        {
            List<Brep> BrepFiltered = new List<Brep>();
            for (int i = 0; i < Count; i++)
            {
                Curve[] CutCrvs = Curve.ProjectToBrep(FootprintCurve, MergedBreps[i], Rhino.Geometry.Vector3d.ZAxis, MTolerance);
                Brep[] BrepSplit = MergedBreps[i].Split(CutCrvs, MTolerance);
                foreach (Brep br in BrepSplit)
                {
                    AreaMassProperties ba = AreaMassProperties.Compute(br);
                    if (FootprintCurve.Contains(ba.Centroid, Rhino.Geometry.Plane.WorldXY, MTolerance) == PointContainment.Inside)
                    {
                        BrepFiltered.Add(br);
                    }
                }
            }
            return BrepFiltered.ToArray();
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Get footprint as polylinecurve and slope degree info for roof
            Curve InFootprint = null;
            Double SlopeAngle = new double();
            Double MythFactor = new double();
            if (!DA.GetData(0, ref InFootprint)) return;
            if (!DA.GetData(1, ref SlopeAngle)) return;
            if (!DA.GetData(2, ref MythFactor)) return;

            if (SlopeAngle <= 0 || SlopeAngle >=90)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Slope degree must be: 0 < deg < 90");
                return;
            }
            if (MythFactor <= 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Myth factor cannot be 0 or negative");
                return;
            }

            //Create Roof Base
            Brep RoofBase = Brep.CreatePlanarBreps(InFootprint, MTolerance)[0];
            if(RoofBase == null)
            {
                return;
            }

            //Main Calculation

            SlopeRad = -(SlopeAngle * Math.PI) / 180;
            LineLength = InFootprint.GetLength()*MythFactor/Math.Cos(SlopeRad);
            PolylineCurve RawRoofPolylineCurve = new PolylineCurve();
            try
            {
                RawRoofPolylineCurve = InFootprint.Simplify(CurveSimplifyOptions.All, MTolerance, Rhino.RhinoDoc.ActiveDoc.ModelAngleToleranceRadians) as PolylineCurve;
            }
            catch
            {
                return;
            }

            //Unify footprint polylinecurve
            CommonFunctions CF = new CommonFunctions();
            PolylineCurve PolylineCurveUnified = CF._polylinecurve_clockwise(RawRoofPolylineCurve, MTolerance);
            if (PolylineCurveUnified == null)
            {
                return;
            }

            //Get roof edges, apply slope, calculate roof intersection
            Line[] EdgeLines = RawRoofPolylineCurve.ToPolyline().GetSegments();
            EdgeCount = EdgeLines.Length;
            DA.SetDataList(0, EdgeLines);

            //Get Slope Vectors
            Vector3d[] SlopeVecs = GetSlopeVector(EdgeLines, EdgeCount, SlopeRad);
            if(SlopeVecs == null)
            {
                return;
            }
            DA.SetDataList(1, SlopeVecs);

            //Get Hip Vectors
            Vector3d[] HipVectors = GetHipVector(EdgeLines, SlopeVecs, EdgeCount);
            if(HipVectors == null)
            {
                return;
            }
            DA.SetDataList(2, HipVectors);

            //Get Roof Breps Untrimmed
            Brep[] RoofBrepsUntrimmed = GetUntrimmedRoofBreps(EdgeLines, HipVectors, EdgeCount, LineLength);
            if (RoofBrepsUntrimmed == null)
            {
                return;
            }
            DA.SetDataList(3, RoofBrepsUntrimmed);
            Brep[] MergedBreps = CleanedUntrimmedBreps(RoofBrepsUntrimmed, EdgeCount);
            if (MergedBreps == null)
            {
                return;
            }
            DA.SetDataList(4, MergedBreps);

            /*Brep[] TrimmedBreps = GetBrepTrimmed(MergedBreps, RawRoofPolylineCurve, EdgeCount);
            if(TrimmedBreps == null)
            {
                return;
            }
            DA.SetDataList(4, TrimmedBreps);*/

            List<Brep> RoofFacesReady = MergedBreps.ToList();
            RoofFacesReady.Add(RoofBase);
            foreach(Brep br in RoofFacesReady)
            {
                foreach (BrepFace bf in br.Faces)
                {
                    bf.ShrinkFace(BrepFace.ShrinkDisableSide.ShrinkAllSides);
                    bf.ShrinkSurfaceToEdge();
                }
                br.MergeCoplanarFaces(MTolerance);
            }

            Brep[] SuperRoof = Brep.CreateSolid(RoofFacesReady, MTolerance);
            if(SuperRoof == null)
            {
                return;
            }
            if (SuperRoof.Length > 0)
            {
                DA.SetData(5, SuperRoof[0]);
            }
            else
            {
                DA.SetData(5, null);
            }
            
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.FootprintToRoof;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("0b86006c-071f-4a27-9c40-5b34b94a7989"); }
        }
    }
}
