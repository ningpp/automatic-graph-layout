using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Routing.Spline.ConeSpanner;

namespace Microsoft.Msagl.Routing.Rectilinear.Nudging {
    public class AxisEdgeHighPointEvent : SweepEvent {
        Point site;
        public AxisEdge AxisEdge { get; set; }

        public AxisEdgeHighPointEvent(AxisEdge edge, Point point) {
            site = point;
            AxisEdge = edge;
        }

        public override Point Site {
            get { return site; }
        }


    }
}