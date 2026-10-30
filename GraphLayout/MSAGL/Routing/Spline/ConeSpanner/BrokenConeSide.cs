using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;

namespace Microsoft.Msagl.Routing.Spline.ConeSpanner{
    /// <summary>
    /// represents a cone side that is broken by the obstacle 
    /// </summary>
    public class BrokenConeSide:ConeSide {
        /// <summary>
        /// point where it starts
        /// </summary>
        public Point start;

        public override Point Start {
            get { return start; }
        }

        /// <summary>
        /// it is the side of the cone that intersects the obstacle side
        /// </summary>
        public ConeSide ConeSide { get; set; }

        public PolylinePoint EndVertex { get; set; }

        public Point End {
            get { return EndVertex.Point; }
        }
      

        public BrokenConeSide(Point start, PolylinePoint end, ConeSide coneSide) {
            this.start = start;
            EndVertex = end;
            ConeSide = coneSide;
        }


        public override Point Direction {
            get { return End-Start; }
        }

        public override string ToString() {
            return "BrokenConeSide: " + Start + "," + End;
        }
    }
}
