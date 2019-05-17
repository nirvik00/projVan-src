using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace ProjVan
{
    public class ProjVanComponent : GH_Component
    {
        public ProjVanComponent()
          : base("ProjVan", "PV",
              "initial inputs from rhino provided by the user",
              "PW19", "vanUfg")
        {
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("input site curves", " iSites", "input: all site curves", GH_ParamAccess.list);
            pManager.AddNumberParameter("input depth of setback", "idepth", "input: setback curve", GH_ParamAccess.item);
            pManager.AddNumberParameter("input for floor Ht", "iht", "input for floor-floor ht", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("output site curves", "oSites", "site curves", GH_ParamAccess.list);
            pManager.AddCurveParameter("output site setback curves", "oSetback", "setback site curves", GH_ParamAccess.list);
            pManager.AddPointParameter("output site setback curve - points", "oSPts", "output: all site curves- points", GH_ParamAccess.list);
            pManager.AddCurveParameter("output CRV", "oCRV", "output floor Curves", GH_ParamAccess.list);
            pManager.AddBrepParameter("output Brep", "oBrep", "output brep Curves", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> siteCrvs = new List<Curve>();
            double setbackDist = 0.0;
            double flrHt = 2.5;

            if (!DA.GetDataList(0, siteCrvs)) return; // site curve inputs

            if (!DA.GetData(1, ref setbackDist)) return; // get setback input
            if (setbackDist < 0.01) return; // avoid <= 0 setback error

            if (!DA.GetData(2, ref flrHt)) return; // floor-floor height input
            if (flrHt < 0.01) return; // avoid <= 0 setback error

            List<Polyline> polyList = new List<Polyline>();
            List<Curve> crv2List = new List<Curve>();
            for(int i=0; i<siteCrvs.Count; i++)
            {
                siteCrvs[i].TryGetPolyline(out Polyline poly);
                polyList.Add(poly); // first output

                try
                {
                    Plane baseplane = Plane.WorldXY;
                    Curve[] crv2 = siteCrvs[i].Offset(baseplane, setbackDist, 0.0, 0);
                    if (crv2.Length > 1)
                    {
                        crv2 = siteCrvs[i].Offset(baseplane, -setbackDist, 0.0, 0);
                        crv2List.AddRange(crv2);
                    }
                }
                catch (Exception)
                {

                }
                
            }
            DA.SetDataList(0, polyList); // copy the site curve - output 

            if (crv2List.Count < 1) return;
            DA.SetDataList(1, crv2List);

            GenBaseCrvs genbasecrvs = new GenBaseCrvs(crv2List, flrHt);
            List<Point3d> pt2List = genbasecrvs.getPts();
            DA.SetDataList(2, pt2List);
            List<Curve> poly2List = genbasecrvs.getCrvs();//each floor plate 
            DA.SetDataList(3, poly2List);

            List<Brep> brepLi = new List<Brep>();
            brepLi=genbasecrvs.GetSrf();
            DA.SetDataList(4, brepLi);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("9ca53ad7-ff5b-4e44-b7ed-d8701beef001"); }
        }
    }
}
