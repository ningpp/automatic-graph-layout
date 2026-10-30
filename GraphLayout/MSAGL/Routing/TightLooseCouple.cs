﻿using System;
using Microsoft.Msagl.Core.Geometry.Curves;

namespace Microsoft.Msagl.Routing {
    /// <summary>
    /// an utility class to keep different polylines created around a shape
    /// </summary>
    public class TightLooseCouple {
        public Polyline TightPolyline { get; set; }
        public Shape LooseShape { get; set; }

        public TightLooseCouple() { }

        public TightLooseCouple(Polyline tightPolyline, Shape looseShape, double distance) {
            TightPolyline = tightPolyline;
            LooseShape = looseShape;
            Distance = distance;
        }
        /// <summary>
        /// the loose polyline has been created with this distance
        /// </summary>
        public double Distance { get; set; }
        public override string ToString() {
            return Prefix(TightPolyline) + "," + Prefix(LooseShape);
        }

        static string Prefix(object value) {
            if (value == null)
                return "null";
            var text = value.ToString();
            return text == null ? "null" : text.Substring(0, Math.Min(5, text.Length));
        }
    }
}
