using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Zachitect_GH
{
    public class DataDict : GH_Component
    {

        public DataDict()
          : base(
                "Dictionary",//Full Name
                "Dict",//Nick Name
                "Create a dictionary by keys (strings or numbers) and values and provide access to values based on key inputs.\n\n© Zach X.G. Zheng | Zachitect.com",//Description
                "Zachitect.com",//Tab
                "Data"//Category
                )
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Dict Keys", "Keys", "keys (strings or numbers) of dictionary, cannot have duplicated items", GH_ParamAccess.list);
            pManager.AddGenericParameter("Dict Values", "Values", "Values of dictionary, can have duplicated items", GH_ParamAccess.list);
            pManager.AddGenericParameter("Input Keys", "In Keys", "Input keys to access values", GH_ParamAccess.list);
            //pManager.AddGenericParameter("Input Values", "InValues", "Input values to access keys", GH_ParamAccess.list);
            pManager[2].Optional = true;
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Result Values", "Out Values", "Values of input keys", GH_ParamAccess.list);
            //pManager.AddGenericParameter("Result Keys", "OutKeys", "Keys of input values", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Object> KeyObjects = new List<Object>();
            if (!DA.GetDataList(0, KeyObjects)) return;
            List<Object> ValueObjects = new List<Object>();
            if (!DA.GetDataList(1, ValueObjects)) return;
            List<Object> InputKeys = new List<Object>();
            DA.GetDataList(2, InputKeys);

            bool NotPure = false;
            List<String> ProcessedString = new List<String>();
            foreach(object obj in KeyObjects)
            {
                if(GH_Convert.ToString(obj, out String str, GH_Conversion.Both))
                {
                    ProcessedString.Add(str);
                }
                else
                {
                    NotPure = true;
                }
            }

            bool InputNotPure = false;
            List<String> ProcessedInput = new List<String>();
            if (InputKeys.Count > 0)
            {
                foreach (object obj in InputKeys)
                {
                    if (GH_Convert.ToString(obj, out String str, GH_Conversion.Both))
                    {
                        ProcessedInput.Add(str);
                    }
                    else
                    {
                        InputNotPure = true;
                    }
                }
            }

            if (NotPure)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "keys contain data types other than string and numbers");
                return;
            }

            if (InputNotPure)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input keys contain data types other than string and numbers");
                return;
            }

            if (ProcessedString.Count != new HashSet<String>(ProcessedString).Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "There are duplicated keys, please make sure keys are unique like talented yourself");
                return;
            }
            if (KeyObjects.Count != ValueObjects.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Keys and Values are of different numbers, please make sure they are in pairs as they wish");
                return;
            }


            Dictionary<String, Object> DICT = new Dictionary<String, Object>();
            for(int i =0; i < ProcessedString.Count; i++)
            {
                DICT.Add(ProcessedString[i], ValueObjects[i]);
            }

            if (ProcessedInput.Count > 0)
            {
                List<Object> OutValue = new List<object>();
                foreach(String o in ProcessedInput)
                {
                    Object outvalue;
                    DICT.TryGetValue(o, out outvalue);
                    OutValue.Add(outvalue);
                }
                DA.SetDataList(0, OutValue);
            }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.DataDict;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("2e077831-ab0a-4d56-9801-43d24f54d5ed"); }
        }
    }
}
