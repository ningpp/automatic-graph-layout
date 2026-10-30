
namespace Microsoft.Msagl.Core.Geometry {
    /// <summary>
    /// A point of a convex hull computation.
    /// </summary>
    public class HullPoint {
        Point point;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        /// <summary>
        /// The point coordinates.
        /// </summary>
        public Point Point {
            get { return point; }
            set { point = value; }
        }
        bool deleted;

        /// <summary>
        /// True if the point was deleted as an inner collinear point.
        /// </summary>
        public bool Deleted {
            get { return deleted; }
            set { deleted = value; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        /// <summary>
        /// Constructs a hull point.
        /// </summary>
        public HullPoint(Point point) {
            this.Point = point;
        }

        public override string ToString() {
            return point + (Deleted ? "X" : "");
        }
    }
}
