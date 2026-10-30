using Microsoft.Msagl.Core.Geometry;

namespace Microsoft.Msagl.Routing.Spline.ConeSpanner {
    public class PortLocationEvent : SweepEvent {
        public PortLocationEvent(Point portLocation) {
            PortLocation = portLocation;
        }

        public override Point Site {
            get {return PortLocation; }
        }

        protected Point PortLocation { get; set; }
    }
}