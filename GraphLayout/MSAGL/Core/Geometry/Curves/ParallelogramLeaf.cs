using System;

namespace Microsoft.Msagl.Core.Geometry.Curves {

    /// <summary>
    /// A leaf of the ParallelogramNodeOverICurve hierarchy.
    /// Is used in curve intersectons routine.
    /// </summary>
    [Serializable]
    public class ParallelogramLeaf : ParallelogramNodeOverICurve {
        double low;

        public double Low {
            get {
                return low;
            }
            set {
                low = value;
            }
        }
        double high;

        public double High {
            get {
                return high;
            }
            set {
                high = value;
            }
        }

        public ParallelogramLeaf(double low, double high, Parallelogram box, ICurve seg, double leafBoxesOffset)
            : base(seg, leafBoxesOffset) {
            this.low = low;
            this.high = high;
            this.Parallelogram = box;
        }



        LineSegment chord;

        public LineSegment Chord {
            get { return chord; }
            set {
                chord = value;
                if (!ApproximateComparer.Close(Seg[low], chord.Start))
                    throw new InvalidOperationException();
            }
        }
    }
}
