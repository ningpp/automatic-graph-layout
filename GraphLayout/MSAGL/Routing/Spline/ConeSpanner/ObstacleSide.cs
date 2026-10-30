using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Routing.Visibility;

namespace Microsoft.Msagl.Routing.Spline.ConeSpanner
{
         public abstract class ObstacleSide : SegmentBase
    {
        public PolylinePoint StartVertex { get; private set; }
        internal ObstacleSide(PolylinePoint startVertex)
        {
            StartVertex = startVertex;
        }

        public abstract PolylinePoint EndVertex { get; }

        public Polyline Polyline { get { return StartVertex.Polyline; } }

        public override Point Start
        {
            get { return StartVertex.Point; }
        }

        public override Point End
        {
            get { return EndVertex.Point; }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            string typeString = this.GetType().ToString();
            int lastDotLoc = typeString.LastIndexOf('.');
            if (lastDotLoc >= 0) {
                typeString = typeString.Substring(lastDotLoc + 1);
            }
            return typeString + " [" + this.Start.ToString() + " -> " + this.End.ToString() + "]";
        }
    }
}
