//
// Clump.cs
// MSAGL class for accreting clumps of overlapped obstacles for Rectilinear Edge Routing.
//
// Copyright Microsoft Corporation.

using System.Collections.Generic;

namespace Microsoft.Msagl.Routing.Rectilinear {
    /// <summary>
    /// This is the list of obstacles in the clump.
    /// </summary>
    public class Clump : List<Obstacle> {
        public Clump(IEnumerable<Obstacle> obstacles) {
            this.AddRange(obstacles);
        }
        /// <summary>
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        public override string ToString() {
            return string.Format("({0}", this.Count);
        }

    }
}
