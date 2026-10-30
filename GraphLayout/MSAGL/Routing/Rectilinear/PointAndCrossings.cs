//
// PointAndCrossings.cs
// MSAGL class for a Point and any Group boundary crossings at that Point, for Rectilinear Edge Routing.
//
// Copyright Microsoft Corporation.
using System.Collections.Generic;
using Microsoft.Msagl.Core.Geometry;

namespace Microsoft.Msagl.Routing.Rectilinear {
    public struct PointAndCrossings {
        public Point Location { get; private set; }
        public List<GroupBoundaryCrossing> Crossings { get; private set; }

        public PointAndCrossings(Point loc, List<GroupBoundaryCrossing> crossings) : this() {
            Location = loc;
            Crossings = crossings;
        }
    }
}