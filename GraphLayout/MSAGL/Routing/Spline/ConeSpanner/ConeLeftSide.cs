using Microsoft.Msagl.Core.Geometry;

namespace Microsoft.Msagl.Routing.Spline.ConeSpanner {
    public class ConeLeftSide:ConeSide {
        public ConeLeftSide(Cone cone) { Cone = cone; }

        public override Point Start {
            get { return Cone.Apex; }
        }

        public override Point Direction {
            get { return Cone.LeftSideDirection; }
        }
        public override string ToString() {
            return "ConeLeftSide " + Start + " " + Direction;
        }

    }
}
