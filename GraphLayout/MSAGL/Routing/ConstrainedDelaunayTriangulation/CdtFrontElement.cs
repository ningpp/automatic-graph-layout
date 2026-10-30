using System.Diagnostics;

namespace Microsoft.Msagl.Routing.ConstrainedDelaunayTriangulation {
    public class CdtFrontElement {
        //The LeftSite should coincide with the leftmost end of the Edge, and the edge should not be vertical

        public CdtSite LeftSite;
        public CdtEdge Edge;
        public CdtSite RightSite;

        public double X {
            get { return LeftSite.Point.X; }
        }

        public CdtFrontElement(CdtSite leftSite, CdtEdge edge) {
            Debug.Assert(edge.upperSite.Point.X != edge.lowerSite.Point.X &&
                         edge.upperSite.Point.X < edge.lowerSite.Point.X && leftSite == edge.upperSite ||
                         edge.upperSite.Point.X > edge.lowerSite.Point.X && leftSite == edge.lowerSite);
            RightSite = edge.upperSite == leftSite ? edge.lowerSite : edge.upperSite;
            LeftSite = leftSite;
            Edge = edge;
        }
    }
}