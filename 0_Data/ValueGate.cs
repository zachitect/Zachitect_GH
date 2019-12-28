using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Zachitect_GH
{
    public class ValueGate : GH_Component
    {

        public ValueGate()
          : base(
                "Value Gate",//Full Name
                "V Gate",//Nick Name
                "Return bool results by comparing values against > < = conditions.\n\n© Zach X.G. Zheng | Zachitect.com",//Description
                "Zachitect.com",//Tab
                "Data"//Category
                )
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Values", "Values", "Numeric values to compare against value gates", GH_ParamAccess.list);
            pManager.AddNumberParameter("Min", "Min", "> Minimum", GH_ParamAccess.item);
            pManager.AddNumberParameter("Max", "Max", "< Maximum", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Boolean>>", "Bool>>", "Boolean result for value containment without equal (A > X > B)", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Boolean>=", "Bool>=", "Boolean result for value containment with equal (A ≥ X ≥ B)", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Double> InputValues = new List<Double>();
            if (!DA.GetDataList(0, InputValues)) return;
            Double MinGate = new Double();
            if (!DA.GetData(1, ref MinGate)) return;
            Double MaxGate = new Double();
            if (!DA.GetData(2, ref MaxGate)) return;

            if (MinGate > MaxGate)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Min value cannot be larger than max value, this is common sense");
                return;
            }

            bool[] BoolResult = new bool[InputValues.Count];
            bool[] BoolResultII = new bool[InputValues.Count];

            for (int i =0; i<InputValues.Count;i++)
            {
                if(InputValues[i] > MinGate && InputValues[i] < MaxGate)
                {
                    BoolResult[i] = true;
                }
                else
                {
                    BoolResult[i] = false;
                }

                if (InputValues[i] >= MinGate && InputValues[i] <= MaxGate)
                {
                    BoolResultII[i] = true;
                }
                else
                {
                    BoolResultII[i] = false;
                }
            }
            DA.SetDataList(0, BoolResult);
            DA.SetDataList(1, BoolResultII);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.ValueGate;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("c96cb90f-aca1-47fc-8382-97410d63a691"); }
        }
    }
}
