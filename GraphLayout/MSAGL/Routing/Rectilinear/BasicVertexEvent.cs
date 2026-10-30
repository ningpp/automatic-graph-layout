//
// BasicVertexEvent.cs
// MSAGL Base class for vertex events for Rectilinear Edge Routing.
//
// Copyright Microsoft Corporation.

using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Routing.Spline.ConeSpanner;

namespace Microsoft.Msagl.Routing.Rectilinear {
    public abstract class BasicVertexEvent : VertexEvent {
        // This is just a subclass to carry the Obstacle object in addition to the Polyline.
        public Obstacle Obstacle { get; private set; }

        public BasicVertexEvent(Obstacle obstacle, PolylinePoint p) : base (p) {
            this.Obstacle = obstacle;
        }
    }
}
