using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;
using System.Diagnostics;

namespace Microsoft.Msagl.Routing.Spline.Bundling {
    public class BundleBase {

        /// <summary>
        /// only one of those is not null
        /// </summary>
        public BundleInfo OutgoingBundleInfo;

        public BundleInfo IncomingBundleInfo;

        readonly Point[] points;

        readonly Point[] tangents;

        public OrientedHubSegment[] OrientedHubSegments;

        /// <summary>
        /// boundary of cluster or hub containing this base 
        /// </summary>
        public ICurve Curve;

        /// <summary>
        /// this bundle base sits on a cluster boundary and the opposite base sits on a child of the cluster 
        /// </summary>
        public bool IsParent;

        /// <summary>
        /// if true then the base sits on a real node or cluster, otherwise it belongs to an intermediate hub
        /// </summary>
        public bool BelongsToRealNode;

        /// <summary>
        /// position of the station containing the base
        /// (could be a center of a hub, or a point on the boundary of a cluster)
        /// </summary>
        public Point Position;

        //need for debug only
        public int stationIndex;

        /// <summary>
        /// constructor
        /// </summary>
        public BundleBase(int count, ICurve boundaryCurve, Point position, bool belongsToRealNode, int stationIndex) {
            BelongsToRealNode = belongsToRealNode;
            Curve = boundaryCurve;
            Position = position;
            this.stationIndex = stationIndex;
            points = new Point[count];
            tangents = new Point[count];
            OrientedHubSegments = new OrientedHubSegment[count];
            ParameterSpan = Curve.ParEnd - Curve.ParStart;
        }

        public Point CurveCenter { get { return Curve.BoundingBox.Center; } }

        public BundleBase OppositeBase { get { return OutgoingBundleInfo != null ? OutgoingBundleInfo.TargetBase : IncomingBundleInfo.SourceBase; } }

        public int Count { get { return points.Length; } }

        public Point[] Points { get { return points; } }

        public Point[] Tangents { get { return tangents; } }

        double initialMidParameter;

        public double InitialMidParameter {
            get { return initialMidParameter; }
            set {
                initialMidParameter = value;
                InitialMidPoint = Curve[value];
            }
        }

        public Point InitialMidPoint { get; set; }

        double parRight;
        /// <summary>
        /// corresponds to the left point of the base
        /// </summary>
        public double ParRight {
            get { return parRight; }
            set {
                parRight = value;
                RightPoint = Curve[parRight];
            }
        }

        double parLeft;
        /// <summary>
        /// corresponds to the right point of the base
        /// </summary>
        public double ParLeft {
            get { return parLeft; }
            set {
                parLeft = value;
                LeftPoint = Curve[parLeft];
            }
        }

        public double ParMid {
            get { return (parRight + parLeft) / 2; }
        }

        public Point RightPoint { get; set; }

        public Point LeftPoint { get; set; }

        public Point MidPoint {
            get { return (RightPoint + LeftPoint) / 2; }
        }

        //previous in ccw order
        public BundleBase Prev;
        //next in ccw order
        public BundleBase Next;

        public double ParameterSpan;

        public double Span { get { return SpanBetweenTwoPoints(parRight, parLeft); } }

        public double SpanBetweenTwoPoints(double right, double left) {
            return (right <= left ? left - right : left - right + ParameterSpan);
        }

        public Point RotateLeftPoint(int rotationOfSourceLeftPoint, double parameterChange) {
            if (rotationOfSourceLeftPoint == 0) return LeftPoint;
            return RotatePoint(rotationOfSourceLeftPoint, parLeft, parameterChange);
        }

        public Point RotateRigthPoint(int rotationOfSourceRightPoint, double parameterChange) {
            if (rotationOfSourceRightPoint == 0) return RightPoint;
            return RotatePoint(rotationOfSourceRightPoint, parRight, parameterChange);
        }

        Point RotatePoint(int rotation, double t, double parameterChange) {
            double change = ParameterSpan * parameterChange;

            t = t + rotation * change;
            t = AdjustParam(t);

            return Curve[t];
        }

        public double AdjustParam(double t) {
            if (t > Curve.ParEnd)
                t = Curve.ParStart + (t - Curve.ParEnd);
            else if (t < Curve.ParStart)
                t = Curve.ParEnd - (Curve.ParStart - t);
            return t;
        }

        public void RotateBy(int rotationOfRightPoint, int rotationOfLeftPoint, double parameterChange) {
            double change = ParameterSpan * parameterChange;
            if (rotationOfRightPoint != 0)
                ParRight = AdjustParam(ParRight + rotationOfRightPoint * change);
            if (rotationOfLeftPoint != 0)
                ParLeft = AdjustParam(ParLeft + rotationOfLeftPoint * change);
        }

        public bool Intersect(BundleBase other) {
            return Intersect(parRight, parLeft, other.parRight, other.parLeft);
        }

        public bool Intersect(double lParRight, double lParLeft, double rParRight, double rParLeft) {
            if (lParRight > lParLeft)
                return Intersect(lParRight, Curve.ParEnd, rParRight, rParLeft) || Intersect(Curve.ParStart, lParLeft, rParRight, rParLeft);

            if (rParRight > rParLeft)
                return Intersect(lParRight, lParLeft, rParRight, Curve.ParEnd) || Intersect(lParRight, lParLeft, Curve.ParStart, rParLeft);

            Debug.Assert(lParRight <= lParLeft);
            Debug.Assert(rParRight <= rParLeft);
            if (ApproximateComparer.LessOrEqual(lParLeft, rParRight)) return false;
            if (ApproximateComparer.LessOrEqual(rParLeft, lParRight)) return false;

            return true;
        }

        public bool RelativeOrderOfBasesIsPreserved(int rotationOfRightPoint, int rotationOfLeftPoint, double parameterChange) {
            double change = ParameterSpan * parameterChange;

            //we do not swap parRight and parLeft
            double rnew = parRight + rotationOfRightPoint * change;
            double lnew = (parRight < parLeft ? parLeft + rotationOfLeftPoint * change : parLeft + ParameterSpan + rotationOfLeftPoint * change);
            if (rnew > lnew) return false;

            //span could not be greater than pi
            if (SpanBetweenTwoPoints(rnew, lnew) > ParameterSpan / 2.0) return false;

            //the base is the only base in the hub
            if (Prev == null) return true;

            //distance between mid points is larger than parameterChange => we can't change the order
            if (SpanBetweenTwoPoints(Prev.ParMid, ParMid) > change && SpanBetweenTwoPoints(ParMid, Next.ParMid) > change)
                return true;

            Point rSoP = RotateLeftPoint(rotationOfLeftPoint, parameterChange);
            Point lSoP = RotateRigthPoint(rotationOfRightPoint, parameterChange);
            Point newMidPoint = (rSoP + lSoP) / 2.0;
            Point curMidPoint = MidPoint;

            //check Prev
            if (Point.GetTriangleOrientation(CurveCenter, Prev.MidPoint, curMidPoint) != Point.GetTriangleOrientation(CurveCenter, Prev.MidPoint, newMidPoint))
                return false;

            //Next
            if (Point.GetTriangleOrientation(CurveCenter, Next.MidPoint, curMidPoint) != Point.GetTriangleOrientation(CurveCenter, Next.MidPoint, newMidPoint))
                return false;

            return true;
        }
    }
}