//
// TransientVisibilityEdge.cs
// MSAGL class for temporary visibility edges for Rectilinear Edge Routing.
//
// Copyright Microsoft Corporation.

namespace Microsoft.Msagl.Routing.Visibility {
    /// <summary>
    /// passing through such an edge does not cost anything
    /// </summary>
    public class TollFreeVisibilityEdge : VisibilityEdge {
        public TollFreeVisibilityEdge(VisibilityVertex source, VisibilityVertex target)
            : this(source, target, 0) { }

        public TollFreeVisibilityEdge(VisibilityVertex source, VisibilityVertex target, double weight)
            : base(source, target, weight) {}
    }
}
