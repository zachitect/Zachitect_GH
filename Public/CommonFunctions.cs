using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Zachitect_GH
{
    class CommonFunctions
    { 
        public Curve _curve_clockwise(Curve ClosedCurve, Double Tolerance)
        {
            if(ClosedCurve.IsClosed)
            {
                bool flip = false;
                Curve testcurve = ClosedCurve.DuplicateCurve();
                //Project Closed Curve to XY
                if (testcurve.IsInPlane(Rhino.Geometry.Plane.WorldXY) == false)
                {
                    testcurve = Curve.ProjectToPlane(testcurve, Rhino.Geometry.Plane.WorldXY);
                }
                else
                {
                    Point3d sp = testcurve.PointAtLength(2.5 * Tolerance);
                    Point3d ep = testcurve.PointAtLength(5 * Tolerance);
                    Vector3d vecSE = new Vector3d(ep - sp);
                    vecSE.Rotate(Math.PI * 0.5, Rhino.Geometry.Vector3d.ZAxis);
                    Point3d testpt = vecSE + sp;
                    flip = testcurve.Contains(testpt, Rhino.Geometry.Plane.WorldXY, Tolerance) == PointContainment.Inside;
                }
                if(flip == true)
                {
                    ClosedCurve.Reverse();
                }
                return ClosedCurve;
            }
            else 
            {
                return null;
            }
        }
        public PolylineCurve _polylinecurve_clockwise(PolylineCurve ClosedCurve, Double Tolerance)
        {
            if (ClosedCurve.IsClosed)
            {
                bool flip = false;
                Curve testcurve = ClosedCurve.DuplicateCurve();
                //Project Closed Curve to XY
                if (testcurve.IsInPlane(Rhino.Geometry.Plane.WorldXY) == false)
                {
                    testcurve = Curve.ProjectToPlane(testcurve, Rhino.Geometry.Plane.WorldXY);
                }
                else
                {
                    Point3d sp = testcurve.PointAtLength(2.5 * Tolerance);
                    Point3d ep = testcurve.PointAtLength(5 * Tolerance);
                    Vector3d vecSE = new Vector3d(ep - sp);
                    vecSE.Rotate(Math.PI * 0.5, Rhino.Geometry.Vector3d.ZAxis);
                    Point3d testpt = vecSE + sp;
                    flip = testcurve.Contains(testpt, Rhino.Geometry.Plane.WorldXY, Tolerance) == PointContainment.Inside;
                }
                if (flip == true)
                {
                    ClosedCurve.Reverse();
                }
                return ClosedCurve;
            }
            else
            {
                return null;
            }
        }
    }
}
