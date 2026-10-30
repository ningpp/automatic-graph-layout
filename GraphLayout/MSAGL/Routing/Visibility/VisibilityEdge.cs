using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Msagl.Core.Geometry;

namespace Microsoft.Msagl.Routing.Visibility {
    /// <summary>
    /// an edge connecting two VisibilityVertices
    /// </summary>
    [DebuggerDisplay("({Source.Point.X} {Source.Point.Y})->({Target.Point.X} {Target.Point.Y}) ({Weight})")]
    public class VisibilityEdge {
        public double LengthMultiplier = 1;

        public VisibilityEdge() { }

        public VisibilityEdge(VisibilityVertex source, VisibilityVertex target, double weight) {
            Debug.Assert(source.Point != target.Point, "Self-edges are not allowed");
            Source = source;
            Target = target;
            Weight = weight;
        }

        public VisibilityEdge(VisibilityVertex source, VisibilityVertex target) : this (source, target, 1.0) {}

        public double Weight { get; private set; }

        public const double DefaultWeight = 1.0;

        /// <summary>
        ///returns true if and only if the edge can be passed
        /// </summary>
        public Func<bool> IsPassable { get; set; }

    
        /// <summary>
        /// edge source point
        /// </summary>
        public Point SourcePoint {
            get { return Source.Point; }
        }
        /// <summary>
        /// edge target point
        /// </summary>
        public Point TargetPoint {
            get { return Target.Point; }
        }


        public VisibilityVertex Source { get; set; }

        public VisibilityVertex Target { get; set; }

        public double Length {
            get { return (SourcePoint-TargetPoint).Length*LengthMultiplier; }
        }

      


        /// <summary>
        /// Rounded representation; DebuggerDisplay shows the unrounded form.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return String.Format(CultureInfo.InvariantCulture, "{0}->{1} ({2})", Source, Target, Weight.ToString("0.0###", CultureInfo.InvariantCulture));
        }

        public VisibilityEdge ReversedClone() {
            return new VisibilityEdge(Target, Source);
        }

        public VisibilityEdge Clone() {
            return new VisibilityEdge(Source, Target);
        }
    }
}