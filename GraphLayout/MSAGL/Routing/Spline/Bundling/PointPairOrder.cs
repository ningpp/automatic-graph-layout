using System.Collections.Generic;

namespace Microsoft.Msagl.Routing.Spline.Bundling {
    public class PointPairOrder {
        //order for node u of edge u->v
        public List<Metroline> Metrolines = new List<Metroline>();
        public bool orderFixed;
        public Dictionary<Metroline, int> LineIndexInOrder;

        public void Add(Metroline metroline) {
            Metrolines.Add(metroline);
        }
    }
}