//
// LowBendVertexEvent.cs
// MSAGL Obstacle class for vertex events on a LowObstacleSide for Rectilinear Edge Routing.
//
// Copyright Microsoft Corporation.

using Microsoft.Msagl.Core.Geometry.Curves;

namespace Microsoft.Msagl.Routing.Rectilinear {
    public class LowBendVertexEvent: BasicVertexEvent {
        public LowBendVertexEvent(Obstacle obstacle, PolylinePoint p) : base(obstacle, p) { }
    }
}
