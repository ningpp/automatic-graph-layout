using System;
using Microsoft.Msagl.Core.DataStructures;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Routing.Visibility;
using System.Diagnostics;

namespace Microsoft.Msagl.Routing.Spline.Bundling {
    [DebuggerDisplay("({SourcePoint.X},{SourcePoint.Y})->({TargetPoint.X},{TargetPoint.Y})")]
    public class SdBoneEdge {
        public readonly VisibilityEdge VisibilityEdge;
        public readonly SdVertex Source;
        public readonly SdVertex Target;
        int numberOfPassedPaths;

        public SdBoneEdge(VisibilityEdge visibilityEdge, SdVertex source, SdVertex target) {
            VisibilityEdge = visibilityEdge;
            Source = source;
            Target = target;
        }

        public Point TargetPoint {
            get { return Target.Point; }
        }

        public Point SourcePoint {
            get { return Source.Point; }
        }

        public bool IsOccupied {
            get { return numberOfPassedPaths > 0; }
        }

        public Set<CdtEdge> CrossedCdtEdges { get; set; }

        public bool IsPassable {
            get {
                return Target.IsTargetOfRouting || Source.IsSourceOfRouting ||
                       VisibilityEdge.IsPassable == null ||
                       VisibilityEdge.IsPassable();
            }
        }

        public void AddOccupiedEdge() {
            numberOfPassedPaths++;
        }

        public void RemoveOccupiedEdge() {
            numberOfPassedPaths--;
            Debug.Assert(numberOfPassedPaths >= 0);
        }
    }
}