using Microsoft.Msagl.Core.Geometry;

namespace Microsoft.Msagl.Routing.Spline.ConeSpanner {
    /// <summary>
    /// this event caused by the intersection of a ObstacleSideSegment and the other cone side of the same cone
    /// when this event happens the cone has to be removed
    /// </summary>
    public class ConeClosureEvent:SweepEvent {
        Cone coneToClose;

        public Cone ConeToClose {
            get { return coneToClose; }
        }
        Point site;

        public override Point Site {
            get { return site; }
        }

        public ConeClosureEvent(Point site, Cone cone) {
            this.site = site;
            this.coneToClose = cone;
        }

        public override string ToString() {
            return "ConeClosureEvent " + site;
        }
    }
}
