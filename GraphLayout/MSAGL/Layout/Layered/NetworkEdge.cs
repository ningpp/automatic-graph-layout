using System;
using Microsoft.Msagl.Core.GraphAlgorithms;

namespace Microsoft.Msagl.Layout.Layered {
    /// <summary>
    /// Differs from IntEdge in containing a flag indicating belonging to the tree
    /// and containing the cut value
    /// </summary>
    public class NetworkEdge : PolyIntEdge {

        public const int Infinity = Int32.MaxValue;
        public NetworkEdge(PolyIntEdge e)
            : base(e.Source, e.Target) {
            Weight = e.Weight;
            Separation = e.Separation;
        }
        public bool inTree;
        int cut = Infinity;

        public int Cut {
            get { return cut; }
            set { cut = value; }
        }
    }

}
