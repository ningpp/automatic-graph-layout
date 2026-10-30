using Microsoft.Msagl.Core.Geometry;

namespace Microsoft.Msagl.Routing.Spline.ConeSpanner {
    /// <summary>
    /// represents a cone side
    /// </summary>
    public abstract class ConeSide {
        public abstract Point Start { get; }
        public abstract Point Direction { get; }
        public Cone Cone { get; set; }
        internal bool Removed { get; set; }
    }
}
