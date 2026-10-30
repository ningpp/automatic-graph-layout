using System.Diagnostics;

namespace Microsoft.Msagl.Routing.ConstrainedDelaunayTriangulation {
    /// <summary>
    /// 
    /// </summary>
    public class PerimeterEdge {
        public CdtSite Start;
        public CdtSite End;
        public PerimeterEdge Prev, Next;
        public CdtEdge Edge;

        public PerimeterEdge(CdtEdge edge) {
            Debug.Assert(edge.CcwTriangle==null || edge.CwTriangle==null);
            Edge = edge;
        }
    }
}