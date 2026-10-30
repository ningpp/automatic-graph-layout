using Microsoft.Msagl.Core.Geometry;

namespace Microsoft.Msagl.Routing.Spline.ConeSpanner {
    public class ConeRightSide:ConeSide {
        public ConeRightSide(Cone cone) {
            this.Cone = cone; 
        }

        public override Point Start {
            get { return Cone.Apex; }
        }

        public override Point Direction {
            get { return Cone.RightSideDirection; }
        }

        public override string ToString() {
            return "ConeRightSide " + Start + " " + Direction;
        }
    }
}
