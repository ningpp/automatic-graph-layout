using System;
using System.Collections.Generic;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Routing.ConstrainedDelaunayTriangulation;
using Microsoft.Msagl.Routing.Visibility;

namespace Microsoft.Msagl.Routing.Spline.Bundling {
    public class SdVertex {
        public VisibilityVertex VisibilityVertex;
        public List<SdBoneEdge> InBoneEdges = new List<SdBoneEdge>();
        public List<SdBoneEdge> OutBoneEdges = new List<SdBoneEdge>();

        public SdVertex Prev {
            get {
                if (PrevEdge == null) return null;
                return PrevEdge.Source == this ? PrevEdge.Target : PrevEdge.Source;
            }
        }

        public SdBoneEdge PrevEdge { get; set; }

        public SdVertex(VisibilityVertex visibilityVertex) {
            VisibilityVertex = visibilityVertex;
        }

        public CdtTriangle Triangle;

        public bool IsSourceOfRouting { get; set; }

        public bool IsTargetOfRouting { get; set; }

        public Point Point { get { return VisibilityVertex.Point; } }

        double cost;

        public double Cost {
            get {
                if (IsSourceOfRouting) return cost;
                return Prev == null ? double.PositiveInfinity : cost;
            }
            set { cost = value; }
        }

        public void SetPreviousToNull() {
            PrevEdge = null;
        }
    }
}