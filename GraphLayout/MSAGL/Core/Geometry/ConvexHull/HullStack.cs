namespace Microsoft.Msagl.Core.Geometry {
    public class HullStack {
        Point hullPoint;

        public Point Point {
            get { return hullPoint; }
            set { hullPoint = value; }
        }
        HullStack next;

        public HullStack Next {
            get { return next; }
            set { next = value; }
        }

        public HullStack(Point hullPoint) {
            this.Point = hullPoint;
        }

        public override string ToString() {
            return hullPoint.ToString();
        }

    }
}
