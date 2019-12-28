using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Zachitect_GH
{
    public class Zachitect_GHInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Zachitect";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return Properties.Resources.Zachitect_GH;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "Grasshopper Plugin Developed by Zach X.G. Zheng\nZachitect.com";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("46caea44-fceb-4e85-afbf-ac384e8ff854");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "Zach X.G. Zheng";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "Zachitect.com";
            }
        }
    }
}
