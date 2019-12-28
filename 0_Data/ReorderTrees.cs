using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Zachitect_GH
{
    public class ReorderTrees : GH_Component
    {

        public ReorderTrees()
          : base(
                "Reorder Branches",//Full Name
                "Reorder",//Nick Name
                "Createing a new data tree by duplicating branches of another data tree based on the order of input paths, this does not preserve the original branch structures.\n\n© Zach X.G. Zheng | Zachitect.com",//Description
                "Zachitect.com",//Tab
                "Data"//Category
                )
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data Tree", "Tree", "Data tree of which branches to be reordered", GH_ParamAccess.tree);
            pManager.AddPathParameter("Reordered Path", "Paths", "Reordered paths for reordering the tree branches", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("New Tree", "New Tree", "New data tree with reordered branches from the old data tree", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Grasshopper.Kernel.Data.GH_Structure<IGH_Goo> InTree = new Grasshopper.Kernel.Data.GH_Structure<IGH_Goo>();
            List<Grasshopper.Kernel.Data.GH_Path> ReBranches = new List<Grasshopper.Kernel.Data.GH_Path>();

            if (!DA.GetDataTree(0, out InTree)) return;
            if (!DA.GetDataList(1, ReBranches)) return;

            Grasshopper.Kernel.Data.GH_Structure<IGH_Goo> OutTree = new Grasshopper.Kernel.Data.GH_Structure<IGH_Goo>();
            for(int i = 0; i<ReBranches.Count;i++)
            {
                if(InTree.Paths.Contains(ReBranches[i]))
                {
                    Grasshopper.Kernel.Data.GH_Path path = new Grasshopper.Kernel.Data.GH_Path(i);
                    foreach (object obj in InTree.get_Branch(ReBranches[i]))
                    {
                        GH_ObjectWrapper GooObj = new GH_ObjectWrapper(obj);
                        OutTree.Append(GooObj, path);
                    }
                }
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input data tree does not contain some of the paths provided, please make sure appropriate paths are supplied");
                    return;
                }

            }
            DA.SetDataTree(0, OutTree);
        }
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.ReorderTrees;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("fd02e678-b610-4851-bbfb-63ac0757880b"); }
        }
    }
}
