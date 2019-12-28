using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Zachitect_GH
{
    public class Place3DPin : GH_Component
    {

        public Place3DPin()
          : base(
                "Place 3D Pin",//Full Name
                "3D Pin",//Nick Name
                "Place a 3D pin onto a point.\n\n© Zach X.G. Zheng | Zachitect.com",//Description
                "Zachitect.com",//Tab
                "Annotation"//Category
                )
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Location", "Location", "Location point to place 3D pin", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Segment", "Segment", "Number of sides for pin polygon", GH_ParamAccess.item);
            pManager.AddNumberParameter("Radius", "Radius", "Radius of the pin", GH_ParamAccess.item);
            pManager.AddNumberParameter("Height", "Height", "Height of the pin", GH_ParamAccess.item);
            pManager.AddNumberParameter("Text Elevation", "Elevation", "A point for text tag above the 3D pin", GH_ParamAccess.item);
            pManager[4].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("3D Pin", "3D Pin", "The placed 3D pin", GH_ParamAccess.item);
            pManager.AddPointParameter("Tag Point", "Tag Pt", "The point above 3D pin for text tag", GH_ParamAccess.item);
        }
        Double MTolerance = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d Location = new Point3d();
            if (!DA.GetData(0, ref Location)) return;
            int Segment = new int();
            if (!DA.GetData(1, ref Segment)) return;
            Double Radius = new Double();
            if (!DA.GetData(2, ref Radius)) return;
            Double Height = new Double();
            if (!DA.GetData(3, ref Height)) return;
            Double Elev = new Double();
            DA.GetData(4, ref Elev);

            if (Segment > 12)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Segment number is too large");
                return;
            }
            if (Segment < 3)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Segment number is too small");
                return;
            }
            if (Radius < MTolerance)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Radius is too small");
                return;
            }
            if (Height < MTolerance)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Height is too small");
                return;
            }
            if (Elev < 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Text tag poing elevation cannot be negative");
                return;
            }

            Rhino.Geometry.Plane PlacePlane = new Plane(Location, Rhino.Geometry.Vector3d.ZAxis);
            Point3d CirclePt = new Point3d(Location.X, Location.Y, Height);
            Point3d TextPt = new Point3d(Location.X, Location.Y, Height + Elev);
            Rhino.Geometry.Circle InscribeCircle = new Circle(CirclePt, Radius);
            Point3d[] PolygonPts = Rhino.Geometry.Polyline.CreateInscribedPolygon(InscribeCircle, Segment).ToArray();

            Brep[] PinFaces = new Brep[PolygonPts.Length * 2];
            for(int i =0; i< PolygonPts.Length-1; i++)
            {
                PinFaces[i] = Brep.CreateFromCornerPoints(PolygonPts[i], PolygonPts[i + 1], Location, MTolerance);
                PinFaces[i + PolygonPts.Length] = Brep.CreateFromCornerPoints(PolygonPts[i], PolygonPts[i + 1], CirclePt, MTolerance);
            }
            Brep ThePin = Brep.JoinBreps(PinFaces, MTolerance)[0];

            DA.SetData(0, ThePin);
            DA.SetData(1, TextPt);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.Place3DPin;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("48da4180-b587-45fe-bea5-e51d5d167cc0"); }
        }
    }
}
