using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;

namespace Microsoft.Msagl.Routing.Spline.ConeSpanner {
    public class Cone {
        bool removed;

        public bool Removed {
            get { return removed; }
            set { removed = value; }
        }
        Point apex;
        readonly IConeSweeper coneSweeper;

        public Cone(Point apex, IConeSweeper coneSweeper) {
            this.apex = apex;
            this.coneSweeper = coneSweeper;
        }

        public Point Apex {
            get { return apex; }
            set { apex = value; }
        }

        public Point RightSideDirection {
            get { return coneSweeper.ConeRightSideDirection; }
        }

        public Point LeftSideDirection {
            get { return coneSweeper.ConeLeftSideDirection; }
        }



        private ConeSide rightSide;

        public ConeSide RightSide {
            get { return rightSide; }
            set { rightSide = value;
            rightSide.Cone = this;
            }
        }
        private ConeSide leftSide;

        public ConeSide LeftSide {
            get { return leftSide; }
            set { 
                leftSide = value;
                leftSide.Cone = this;
            }
        }
    }
}
