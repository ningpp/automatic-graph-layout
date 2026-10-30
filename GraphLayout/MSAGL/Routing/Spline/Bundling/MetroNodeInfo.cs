using Microsoft.Msagl.Core.Geometry.Curves;

namespace Microsoft.Msagl.Routing.Spline.Bundling {
    public class MetroNodeInfo {
        Metroline metroline;
        Station station;
        PolylinePoint polyPoint;

        public MetroNodeInfo(Metroline metroline, Station station, PolylinePoint polyPoint) {
            this.metroline = metroline;
            this.station = station;
            this.polyPoint = polyPoint;
        }

        public Metroline Metroline {
            get { return metroline; }
        }

        public PolylinePoint PolyPoint {
            get { return polyPoint; }
        }
    }
}