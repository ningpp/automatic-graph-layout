using System.Collections.Generic;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;

namespace Microsoft.Msagl.Routing.Spline.Bundling {
    public class StationEdgeInfo {

        public int Count {get {return this.Metrolines.Count;} }
        public double Width;

        public List<Metroline> Metrolines = new List<Metroline>();

        #region cache

        internal double cachedBundleCost;

        //internal Polyline cachedBoundary;

        #endregion
    }
}