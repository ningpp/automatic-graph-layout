using Microsoft.Msagl.Core.GraphAlgorithms;

namespace Microsoft.Msagl.Core.Layout {
    public class SimpleIntEdge : IEdge
    {
        public int Source { get; set; }
        public int Target { get; set; }
    }
}