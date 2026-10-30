using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;

namespace Microsoft.Msagl.Routing {
    public class RelaxedPolylinePoint  {
        private PolylinePoint polylinePoint;

        public PolylinePoint PolylinePoint {
            get { return polylinePoint; }
            set { polylinePoint = value; }
        }
        private Point originalPosition;

        public Point OriginalPosition {
            get { return originalPosition; }
            set { originalPosition = value; }
        }
        
        public RelaxedPolylinePoint(PolylinePoint polylinePoint, Point originalPosition) {
            this.PolylinePoint = polylinePoint;
            this.OriginalPosition = originalPosition;
        }

        RelaxedPolylinePoint next;

        public RelaxedPolylinePoint Next {
            get { return next; }
            set { next = value; }
        }
        RelaxedPolylinePoint prev;

        public RelaxedPolylinePoint Prev {
            get { return prev; }
            set { prev = value; }
        }
    }
}
