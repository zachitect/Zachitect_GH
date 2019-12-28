using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Zachitect_GH
{
    public class RemapValues : GH_Component
    {

        public RemapValues()
          : base(
                "Remap Values",//Full Name
                "Remap V",//Nick Name
                "Remap a list of values to another domain from the bounding domain of input list\n\n© Zach X.G. Zheng | Zachitect.com",//Description
                "Zachitect.com",//Tab
                "Data"//Category
                )
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Values to Map", "Values", "A list of values to remap", GH_ParamAccess.list);
            pManager.AddNumberParameter("Domain Start", "Start", "Default = 0, Start of the domain to remap values to", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Domain End", "End", "Default = 1, End of the domain to remap values to", GH_ParamAccess.item, 1);
            pManager.AddIntervalParameter("Source(Optional)", "Source", "(Optional) source domain", GH_ParamAccess.item);
            pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Remapped Values", "Remapped", "Remapped values from bounding (or source) domain to target domain", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Double> InputValues = new List<Double>();
            Double low2 = new Double();
            Double high2 = new Double();

            if (!DA.GetDataList(0, InputValues)) return;
            if (!DA.GetData(1, ref low2)) return;
            if (!DA.GetData(2, ref high2)) return;

            Interval Source = new Interval();
            DA.GetData(3, ref Source);
            Double low1 = Source.T0;
            Double high1 = Source.T1;

            if(Source.T0 == 0 && Source.T1 == 0)
            {
                List<Double> ProcessedValue = new List<Double>(InputValues);
                ProcessedValue.Sort();
                low1 = ProcessedValue.First();
                high1 = ProcessedValue.Last();
            }

            if (InputValues.Count == 1 && low1 == high1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "There is only one input value, please provide a source domain with different start and end values");
                return;
            }

            if (low1 == high1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Souce domain start and end values are identical, or supplied input values are identical, please provide a source domain with different start and end values");
                return;
            }

            if (low2 == high2)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Target domain start and end values are identical, please provide a target domain with different start and end values");
                return;
            }

            bool valueout = false;
            foreach(Double value in InputValues)
            {
                if(value < low1 || value > high1)
                {
                    valueout = true;
                }
            }

            if (valueout == true)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Values are outside the supplied source domain, please make sure all values are within source domain interval");
                return;
            }

            List<Double>RemappedValues = new List<Double>();
            foreach(Double value in InputValues)
            {
                Double RemappedValue = low2 + (value - low1) * (high2 - low2) / (high1 - low1);
                RemappedValues.Add(RemappedValue);
            }

            DA.SetDataList(0, RemappedValues);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.RemapValues;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("cf115d4e-0dd3-441a-bccc-98083517443f"); }
        }
    }
}
