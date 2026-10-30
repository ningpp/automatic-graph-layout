﻿using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;

namespace Microsoft.Msagl.Routing.Spline.Bundling {
    public class OrientedHubSegment {
        private ICurve segment;
        public bool Reversed;
        public int Index;
        public BundleBase BundleBase;

        public OrientedHubSegment(ICurve seg, bool reversed, int index, BundleBase bundleBase) {
            Segment = seg;
            Reversed = reversed;
            Index = index;
            BundleBase = bundleBase;
        }

        public Point this[double t] { get { return Reversed ? Segment[Segment.ParEnd - t] : Segment[t]; } }

        public OrientedHubSegment Other { get; set; }
        public ICurve Segment {
            get { return segment; }
            set {
                segment = value;
            }
        }
    }
}