using System;
using System.Collections.Generic;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;

namespace Microsoft.Msagl.Routing.Visibility {
    /// <summary>
    /// represents a chunk of a hole boundary
    /// </summary>
    public class Stem {
        PolylinePoint start;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public PolylinePoint Start {
            get { return start; }
            set { start = value; }
        }
        PolylinePoint end;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public PolylinePoint End {
            get { return end; }
            set { end = value; }
        }
     
        
        public Stem(PolylinePoint start, PolylinePoint end) {
            System.Diagnostics.Debug.Assert(start.Polyline==end.Polyline);
            this.start = start;
            this.end = end;
        }

        public IEnumerable<PolylinePoint> Sides {
            get {
                PolylinePoint v=start;
                
                while(v!= end ) {
                    PolylinePoint side = v;
                    yield return side;
                    v = side.NextOnPolyline;
                }
            }
        }

        public bool MoveStartClockwise() {
            if (Start != End) {
                Start = Start.NextOnPolyline;
                return true;
            }
            return false;
        }

        public override string ToString() {
            return String.Format(System.Globalization.CultureInfo.InvariantCulture, "Stem({0},{1})", Start, End);
        }

    }
}
