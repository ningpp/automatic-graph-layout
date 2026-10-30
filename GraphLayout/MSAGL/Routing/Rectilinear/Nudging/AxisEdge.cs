using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Msagl.Core.DataStructures;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Routing.Visibility;

namespace Microsoft.Msagl.Routing.Rectilinear.Nudging {
    /// <summary>
    /// a wrapper arownd VisibilityEdge representing the same edge 
    /// but oriented along the X or the Y axis
    /// </summary>
    public class AxisEdge:VisibilityEdge {
        public Direction Direction { get; set; }
        public AxisEdge(VisibilityVertex source, VisibilityVertex target)
            : base(source, target){
            RightBound = double.PositiveInfinity;
            LeftBound = double.NegativeInfinity;
            Direction = CompassVector.DirectionsFromPointToPoint(source.Point, target.Point);
            Debug.Assert(Direction == Direction.East || Direction == Direction.North);
        }

        readonly public Set<AxisEdge> RightNeighbors = new Set<AxisEdge>();

        public void AddRightNeighbor(AxisEdge edge) {
            RightNeighbors.Insert(edge);
        }

        public double LeftBound { get; set; }

        public double RightBound { get; private set; }

        readonly Set<LongestNudgedSegment> setOfLongestSegs = new Set<LongestNudgedSegment>();
        
        public IEnumerable<LongestNudgedSegment> LongestNudgedSegments { get { return setOfLongestSegs; } }
        
        public void AddLongestNudgedSegment(LongestNudgedSegment segment) {
            setOfLongestSegs.Insert(segment);
        }

        public void BoundFromRight(double rightbound) {
            rightbound = Math.Max(rightbound, LeftBound);
            RightBound = Math.Min(rightbound, RightBound);
        }

        public void BoundFromLeft(double leftbound) {
            leftbound = Math.Min(leftbound, RightBound);
            LeftBound = Math.Max(leftbound, LeftBound);
        }
    }
}