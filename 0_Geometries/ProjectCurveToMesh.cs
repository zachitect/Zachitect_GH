using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Zachitect_GH
{
    public class ProjectCurveToMesh : GH_Component
    {

        public ProjectCurveToMesh()
          : base(
                "Project Curves",//Full Name
                "Project",//Nick Name
                "Project curve(s) onto a mesh.\n\n© Zach X.G. Zheng | Zachitect.com",//Description
                "Zachitect.com",//Tab
                "Geometries"//Category
                )
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Target Mesh", "Mesh", "Mesh to project curve(s) onto", GH_ParamAccess.item);
            pManager.AddCurveParameter("Curve(s)", "Curve(s)", "Curve(s) to be projected", GH_ParamAccess.list);
            pManager.AddVectorParameter("Vector", "Vector", "Vector for projection", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Projected Curve(s)", "Result(s)", "Projected curves", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh TargetMesh = new Mesh();
            if (!DA.GetData(0, ref TargetMesh)) return;
            List<Curve> InputCurves = new List<Curve>();
            if (!DA.GetDataList(1, InputCurves)) return;
            Vector3d PrjVec = new Vector3d();
            if (!DA.GetData(2, ref PrjVec)) return;

            Grasshopper.Kernel.Data.GH_Structure<GH_Curve> outTreeNode = new Grasshopper.Kernel.Data.GH_Structure<GH_Curve>();
            for(int i = 0; i<InputCurves.Count;i++)
            {
                Grasshopper.Kernel.Data.GH_Path path = new Grasshopper.Kernel.Data.GH_Path(i);
                Curve[] CRVS = Curve.ProjectToMesh(InputCurves[i], TargetMesh, PrjVec, MTolerance);
                foreach (Curve crv in CRVS)
                {
                    GH_Curve ghcrv = new GH_Curve(crv);
                    outTreeNode.Append(ghcrv, path);
                }
            }
            DA.SetDataTree(0, outTreeNode);
        }
        Double MTolerance = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.ProjectCurveToMesh;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("786ab670-4ca8-4103-9b6a-e6c7df9d0c7c"); }
        }
    }
}
