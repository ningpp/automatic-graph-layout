using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Layout;

namespace Microsoft.Msagl.Routing.Rectilinear.Nudging {
    /// <summary>
    /// represents the path for an EdgeGeometry 
    /// </summary>
    public class Path {
        /// <summary>
        /// the corresponding edge geometry
        /// </summary>
        public EdgeGeometry EdgeGeometry { get; set; }
        /// <summary>
        /// the path points
        /// </summary>
        public IEnumerable<Point> PathPoints { get; set; }

        public double Width { get { return EdgeGeometry.LineWidth;} }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="edgeGeometry"></param>
        public Path(EdgeGeometry edgeGeometry) {
            EdgeGeometry = edgeGeometry;
        }


        public Point End {
            get {
                return LastEdge.Target;
            }
        }

        public Point Start {
            get {
                return FirstEdge.Source;
            }
        }

        
        public IEnumerable<PathEdge> PathEdges {
            get {
                for (var e = FirstEdge; e != null; e = e.Next)
                    yield return e;
            }
        }

        public PathEdge FirstEdge { get; set; }

        public PathEdge LastEdge { get; set; }


        public void AddEdge(PathEdge edge) {
            edge.Path = this;
            Debug.Assert(edge.Source == LastEdge.Target);
            LastEdge.Next = edge;
            edge.Prev = LastEdge;
            LastEdge = edge;
        }

        public void SetFirstEdge(PathEdge edge) {
            LastEdge = FirstEdge = edge;
            edge.Path = this;
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}->{1}", this.EdgeGeometry.SourcePort.Location, this.EdgeGeometry.TargetPort.Location);
        }
    }
}