using Microsoft.Msagl.Core.Geometry;

namespace Microsoft.Msagl.Routing.Visibility {
    public abstract class SegmentBase {
        public abstract Point Start { get; }
        public abstract Point End { get; }
        public Point Direction { get { return End - Start; } }
        public override string ToString() {
            return Start + " " + End;
        }

    }
}