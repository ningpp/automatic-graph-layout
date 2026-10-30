using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;

namespace Microsoft.Msagl.Routing.Rectilinear.Nudging {
    /// <summary>
    /// A place holder for an edge in a path to keep it inside of a linked list representing a path.
    /// Each PathEdge belongs to only one path
    /// </summary>
    /// PathEdge passes through the AxisEdge that it points to but may go to the different direction.
    /// In the last case the PathEdge is marked as Reversed. Several PathEdges can share the same AxisEdge.
    public class PathEdge {

        public AxisEdge AxisEdge { get; set; }
        public PathEdge Next { get; set; }
        public PathEdge Prev { get; set; }
        public double Width { get; set; }
        public Path Path { get; set; }
        
        public override string ToString() {
            return Source + " " + Target;
        }



        public PathEdge(AxisEdge edgeForNudging, double width) {
            AxisEdge = edgeForNudging;
            Width = width;
        }

   
        LongestNudgedSegment longestNudgedSegment;

        /// <summary>
        /// It is the offset of the edge from the underlying line segment 
        /// [VisibilityEdge.SourcePoint, VisibilityEdge.TargetPoint] in to the direction of the VisibilityEdge.Perpendicular.
        /// Offset holder is the same for the maximal parallel sequence of connected PathEdges
        /// </summary>
        public LongestNudgedSegment LongestNudgedSegment {
            get { return longestNudgedSegment; }
            set { 
                longestNudgedSegment = value;
                if (longestNudgedSegment != null){
                    longestNudgedSegment.AddEdge(this);
                    AxisEdge.AddLongestNudgedSegment(longestNudgedSegment);
                }
            }
        }

        /// <summary>
        /// A fixed edge cannot be shifted from its visibility edge; offset is always 0.
        /// Such an edge can be, for example, a terminal edge going to a port. 
        /// </summary>
        public bool IsFixed { get; set; }

        
        public Point Source {
            get { return !Reversed? AxisEdge.SourcePoint: AxisEdge.TargetPoint; }
        }

        
        public Point Target {
            get { return Reversed ? AxisEdge.SourcePoint : AxisEdge.TargetPoint; }
        }

       
        static public bool VectorsAreParallel(Point a, Point b) {
            return ApproximateComparer.Close(a.X * b.Y - a.Y * b.X, 0);
        }

        public static bool EdgesAreParallel(PathEdge edge, PathEdge pathEdge) {
            return VectorsAreParallel(edge.AxisEdge.TargetPoint-edge.AxisEdge.SourcePoint,
                                      pathEdge.AxisEdge.TargetPoint-pathEdge.AxisEdge.SourcePoint);
        }

        public Direction Direction { get { return Reversed ? CompassVector.OppositeDir(AxisEdge.Direction) : AxisEdge.Direction;} }
        /// <summary>
        /// if set to true then in the path the edge is reversed
        /// </summary>
        public bool Reversed {get; set;}

        int index=-1;//not set yet

        /// <summary>
        /// the index of the edge in the order
        /// </summary>
        public int Index {
            get { return index; }
            set { index = value; }
        }
    }
}