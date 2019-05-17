using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace ProjVan
{
    public class ProjVanInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "ProjVan";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                return null;
            }
        }
        public override string Description
        {
            get
            {
               
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("3c2583ea-be20-49c9-a5d4-2eb50241239b");
            }
        }

        public override string AuthorName
        {
            get
            {
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
