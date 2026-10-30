using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;

namespace Microsoft.Msagl.Routing.Spline.ConeSpanner
{
    public class RightObstacleSide : ObstacleSide
    {
        Point end;
        public RightObstacleSide(PolylinePoint startVertex)
            : base(startVertex)
        {
            this.end = startVertex.PrevOnPolyline.Point;
        }
        public override Point End
        {
            get { return end; }
        }

        public override PolylinePoint EndVertex
        {
            get { return StartVertex.PrevOnPolyline; }
        }

    }
}