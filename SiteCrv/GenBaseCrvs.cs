using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace ProjVan
{
    public class GenBaseCrvs
    {
        List<Curve> baseCrvs;
        List<Curve> flrPlateCrv;
        List<Brep> brepLi;
        double FlrHt = 1.0;
        public GenBaseCrvs(List<Curve> crvs, double flrht)
        {
            baseCrvs = crvs;
            brepLi = new List<Brep>();
            FlrHt = flrht;
        }
        public List<Point3d> getPts()
        {
            List<Point3d> ptList = new List<Point3d>();
            List<Polyline> polyPts=new List<Polyline>();
            for(int i=0; i<baseCrvs.Count; i++)
            {                
                baseCrvs[i].TryGetPolyline(out Polyline poly);
                IEnumerable<Point3d> pts = poly;
                foreach(Point3d p in pts)
                {
                    ptList.Add(p);
                }
                polyPts.Add(poly);
            }
            return ptList;
        }

        public List<Curve> getCrvs()
        {

            List<Curve> polyZLi = new List<Curve>();
            for(int i=0; i<baseCrvs.Count; i++)
            {
                Curve crv = baseCrvs[i];
                baseCrvs[i].TryGetPolyline(out Polyline poly);
                IEnumerable<Point3d> pts = poly;
                int R = 5;
                for(int j=0; j<R; j++)
                {
                    List<Point3d> polyZpts = new List<Point3d>();
                    foreach (Point3d p in pts)
                    {
                        Point3d r = new Point3d(p.X, p.Y, p.Z + FlrHt*j);
                        polyZpts.Add(r);
                    }
                    PolylineCurve polyZ = new PolylineCurve(polyZpts);
                    polyZLi.Add(polyZ);
                }
            }
            flrPlateCrv = new List<Curve>();
            flrPlateCrv = polyZLi;
            return flrPlateCrv;
        }
        public List<Brep> GetSrf()
        {
            brepLi = new List<Brep>();
            for(int i=0; i<flrPlateCrv.Count; i++)
            {
                Curve crv = flrPlateCrv[i];
                Curve crv2 = crv.DuplicateCurve();

                Polyline poly;//do not delete : used implicit-ly
                var t= crv2.TryGetPolyline(out poly);

                IEnumerator<Point3d> ptsEnum = poly.GetEnumerator();
                List<Point3d> ptLi = new List<Point3d>();
                List<Point3d> ptLi2 = new List<Point3d>();
                while (ptsEnum.MoveNext())
                {
                    ptLi.Add(ptsEnum.Current);
                }
                for(int j=0; j<ptLi.Count; j++)
                {
                    Point3d p = ptLi[j];
                    double x = p.X;
                    double y = p.Y;
                    double z = p.Z + FlrHt;
                    ptLi2.Add(new Point3d(x, y, z));
                }
                for(int j=0; j<ptLi2.Count; j++)
                {
                    ptLi.Add(ptLi2[j]);
                }
                Box box = new Box(Plane.WorldXY, ptLi.ToArray());
                Brep brep = Brep.CreateFromBox(box);
                brepLi.Add(brep);
            }
            brepLi.RemoveAll(item => item == null);

            return brepLi;
        }
    } //end class
} //end namespace