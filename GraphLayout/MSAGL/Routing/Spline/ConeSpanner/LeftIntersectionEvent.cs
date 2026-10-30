using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;

namespace Microsoft.Msagl.Routing.Spline.ConeSpanner {
    /// <summary>
    /// left here means an intersection of a left cone side with an obstacle edge
    /// </summary>
    public class LeftIntersectionEvent : SweepEvent {
        public ConeLeftSide coneLeftSide;
        Point intersectionPoint;
        PolylinePoint endVertex;

        public PolylinePoint EndVertex {
            get { return endVertex; }
        }

        public LeftIntersectionEvent(ConeLeftSide coneLeftSide,
            Point intersectionPoint,
            PolylinePoint endVertex) {
            this.coneLeftSide = coneLeftSide;
            this.intersectionPoint = intersectionPoint;
            this.endVertex = endVertex;
        }

        public override Point Site {
            get { return intersectionPoint; }
        }

        public override string ToString() {
            return "LeftIntersectionEvent " + intersectionPoint;
        }
    }
}
