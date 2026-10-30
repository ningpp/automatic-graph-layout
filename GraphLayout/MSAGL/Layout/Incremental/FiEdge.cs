using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.GraphAlgorithms;
using Microsoft.Msagl.Core.Layout;

namespace Microsoft.Msagl.Layout.Incremental {
    /// <summary>
    /// An edge is a pair of nodes and an ideal length required between them
    /// </summary>
    public class FiEdge : IEdge {
        public Edge mEdge;
        public FiNode source;
        public FiNode target;

        public FiEdge(Edge mEdge) {
            this.mEdge = mEdge;
            source = (FiNode) mEdge.Source.AlgorithmData;
            target = (FiNode) mEdge.Target.AlgorithmData;
        }
        #region IEdge Members

        public int Source {
            get { return source.index; }
            set { }
        }

        public int Target {
            get { return target.index; }
            set { }
        }

        #endregion

        public Point vector() {
            return source.mNode.Center - target.mNode.Center;
        }
    }
}