using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;

namespace Microsoft.Msagl.Routing.Spline.ConeSpanner {
    public abstract class VertexEvent: SweepEvent {
        PolylinePoint vertex;

        public PolylinePoint Vertex {
            get { return vertex; }
            set { vertex = value; }
        }

        public override Point Site {
            get { return vertex.Point; }
        }

        public VertexEvent(PolylinePoint p) { vertex = p; }
        public Polyline Polyline { get { return vertex.Polyline; } }
    }
}
