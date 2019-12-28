using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Zachitect_GH
{
    public class DeExtrusion : GH_Component
    {

        public DeExtrusion()
          : base(
                "Deconstruct Extrusion",//Full Name
                "De Extrusion",//Nick Name
                "Deconstruct a vertical solid extrusion to elements and info.\n\n© Zach X.G. Zheng | Zachitect.com",//Description
                "Zachitect.com",//Tab
                "Geometries"//Category
                )
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Extrusion", "Extrusion", "A vertical solid extrusion to deconstruct", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Top Face", "Top", "Top face of the extrusion", GH_ParamAccess.item);
            pManager.AddBrepParameter("Base Face", "Base", "Base face of the extrusion", GH_ParamAccess.item);
            pManager.AddBrepParameter("Side Faces", "Sides", "Side faces of the extrusion", GH_ParamAccess.list);
            pManager.AddPointParameter("Top Centroid", "Centre T", "Centroid of top face", GH_ParamAccess.item);
            pManager.AddPointParameter("Base Centroid", "Centre B", "Centroid of base face", GH_ParamAccess.item);
            pManager.AddNumberParameter("Top Elevation", "Top Elev", "Z value of centroid on top face", GH_ParamAccess.item);
            pManager.AddNumberParameter("Base Elevation", "Top Elev", "Z value of centroid on base face", GH_ParamAccess.item);
            pManager.AddNumberParameter("Profile Area", "Base Area", "The area of extrusion profile", GH_ParamAccess.item);
            pManager.AddNumberParameter("Extrusion Height", "Height", "Height of the extrusion", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Brep BRP = new Brep();
            if (!DA.GetData(0, ref BRP)) return;
            
            if(BRP.IsSolid == false)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input extrusion is not solid");
                return;
            }
            if (BRP.IsManifold == false)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input extrusion is non-manifold, e.g. one edge shared by 3+ faces");
                return;
            }
            if (BRP.IsValid == false)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input object is not a valid brep");
                return;
            }

            BRP.MergeCoplanarFaces(MTolerance);
            List<Brep> VerticalFaces = new List<Brep>();
            List<BrepFace> ProfileFaces = new List<BrepFace>();
            List<Double> LengthList = new List<Double>();
            foreach(BrepFace f in BRP.Faces)
            {
                Vector3d fnorm = f.NormalAt(0.5, 0.5);
                if(fnorm.IsPerpendicularTo(Rhino.Geometry.Vector3d.ZAxis))
                {
                    VerticalFaces.Add(f.DuplicateFace(false));
                    foreach(int ind in f.AdjacentEdges())
                    {
                        Vector3d vecedge = new Vector3d(BRP.Edges[ind].PointAtStart - BRP.Edges[ind].PointAtEnd);
                        if(vecedge.IsParallelTo(Rhino.Geometry.Vector3d.ZAxis) != 0)
                        {
                            LengthList.Add(vecedge.Length);
                        }
                    }
                }
                else
                {
                    ProfileFaces.Add(f);
                }
            }
            if (ProfileFaces.Count != 2)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input object is not a vertial extrusion, e.g. a surface extruded along the Z-axis");
                return;
            }
            if (Math.Abs(ProfileFaces[0].ToBrep().GetArea() - ProfileFaces[1].ToBrep().GetArea()) > MTolerance)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input object as an extrusion has different top and base profiles, please make sure they are identical");
                return;
            }

            Double[] LengthArr = new HashSet<Double>(LengthList).ToArray();
            if(LengthArr.Length != 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input object as an extrusion has different top and base profiles, please make sure they are identical");
                return;
            }

            BrepFace TopF;
            BrepFace BotF;
            if(ProfileFaces[0].PointAt(0,0).Z > ProfileFaces[1].PointAt(0,0).Z)
            {
                TopF = ProfileFaces[0];
                BotF = ProfileFaces[1];
            }
            else
            {
                TopF = ProfileFaces[1];
                BotF = ProfileFaces[0];
            }

            Brep TBrep = TopF.DuplicateFace(false);
            Brep BBrep = BotF.DuplicateFace(false);
            AreaMassProperties AT = Rhino.Geometry.AreaMassProperties.Compute(TBrep);
            AreaMassProperties AB = Rhino.Geometry.AreaMassProperties.Compute(BBrep);
            DA.SetData(0, TBrep);
            DA.SetData(1, BBrep);
            DA.SetDataList(2, VerticalFaces);
            DA.SetData(3, AT.Centroid);
            DA.SetData(4, AB.Centroid);
            DA.SetData(5, AT.Centroid.Z);
            DA.SetData(6, AB.Centroid.Z);
            DA.SetData(7, AB.Area);
            DA.SetData(8, LengthArr[0]);
        }
        Double MTolerance = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.DeExtrusion;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("c9daf074-056a-4cf1-abbb-7acb99fbfb97"); }
        }
    }
}
