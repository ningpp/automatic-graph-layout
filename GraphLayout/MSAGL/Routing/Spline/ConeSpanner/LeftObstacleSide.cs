using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;

namespace Microsoft.Msagl.Routing.Spline.ConeSpanner {
    public class LeftObstacleSide : ObstacleSide {
        readonly Point end;
        public LeftObstacleSide(PolylinePoint startVertex)
            : base(startVertex) {
            end = startVertex.NextOnPolyline.Point;
        }
        public override Point End {
            get { return end; }
        }

        public override PolylinePoint EndVertex {
            get { return StartVertex.NextOnPolyline; }
        }

    }
}
