using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Routing.Spline.ConeSpanner;

namespace Microsoft.Msagl.Routing.Visibility {
    public class PortObstacleEvent : SweepEvent {
        readonly Point site;

        public PortObstacleEvent(Point site) {
            this.site = site;
        }

        public override Point Site {
            get { return site; }
        }
    }
}