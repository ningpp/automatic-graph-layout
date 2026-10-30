using System;
using Microsoft.Msagl.Core.DataStructures;
using Microsoft.Msagl.Core.Geometry;

namespace Microsoft.Msagl.Routing.Visibility {
    public class Diagonal {

        public override string ToString() {
            return String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1}", Start, End);
        }
        public Point Start {
            get { return leftTangent.End.Point; }
        }

        public Point End {
            get { return rightTangent.End.Point; }
        }

        public Diagonal(Tangent leftTangent, Tangent rightTangent) {
            this.LeftTangent = leftTangent;
            this.RightTangent = rightTangent;
        }

        Tangent leftTangent;

        public Tangent LeftTangent {
            get { return leftTangent; }
            set { leftTangent = value; }
        }
        Tangent rightTangent;

        public Tangent RightTangent {
            get { return rightTangent; }
            set { rightTangent = value; }
        }

        RBNode<Diagonal> rbNode;

        public RBNode<Diagonal> RbNode {
            get { return rbNode; }
            set { rbNode = value; }
        }
    }
}
