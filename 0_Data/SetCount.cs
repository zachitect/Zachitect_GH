using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Zachitect_GH
{
    public class SetCount : GH_Component
    {

        public SetCount()
          : base(
                "Set and Count",//Full Name
                "Set C",//Nick Name
                "Return a sorted set from a list and the frequency of occurance for each item.\n\n© Zach X.G. Zheng | Zachitect.com",//Description
                "Zachitect.com",//Tab
                "Data"//Category
                )
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("List", "List", "Input list to create set", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Sorted Set", "Set", "Sorted set from input list", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Frequency", "Count", "Frequency of occurance of each item", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Double> ObjList = new List<Double>();
            if (!DA.GetDataList(0, ObjList)) return;

            if (ObjList is List<Double> == false)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input list must be of numbers");
                return;
            }

            List<Double> ObjHash = new HashSet<Double>(ObjList).ToList();
            ObjHash.Sort();

            List<int> CountInt = new List<int>();
            foreach(Double obj in ObjHash)
            {
                int count = ObjList.Count(o => o == obj);
                CountInt.Add(count);
            }

            DA.SetDataList(0, ObjHash);
            DA.SetDataList(1, CountInt);

        }
        Double MTolerance = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.SetCount;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("aaac5a1c-ede3-4955-afe1-ee71e184bad5"); }
        }
    }
}
