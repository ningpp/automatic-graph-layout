using Microsoft.Msagl.Core.Geometry.Curves;

namespace Microsoft.Msagl.Routing.Spline.ConeSpanner{
    public class LowestVertexEvent:VertexEvent {
        public LowestVertexEvent(PolylinePoint p) : base(p) { }
    }
}
