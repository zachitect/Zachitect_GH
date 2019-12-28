using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Zachitect_GH
{
    public class ClosedCurveUnify : GH_Component
    {

        public ClosedCurveUnify()
          : base(
                "Unify Closed Curve",//Full Name
                "Unify Curve",//Nick Name
                "Unify a closed curve to clock-wise (control points will be ordered clockwise).\n\n© Zach X.G. Zheng | Zachitect.com",//Description
                "Zachitect.com",//Tab
                "Geometries"//Category
                )
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Closed Curve", "Curve", "Closed curve to be unified clockwise", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Clockwise Curve", "Clockwise Curve", "Unified closed curve of which all control points are ordered clockwise", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve InputCurve = null;
            if (!DA.GetData(0, ref InputCurve)) return;
            if (InputCurve.IsClosed == false)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Curve must be closed");
                return;
            }
            CommonFunctions C = new CommonFunctions();
            Curve Result = C._curve_clockwise(InputCurve, MTolerance);
            DA.SetData(0, Result);
        }
        Double MTolerance = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.ClosedCurveUnify;
            }
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("4f2a3e49-e7c5-4090-9721-498c92ca5c98"); }
        }
    }
}
