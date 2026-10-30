using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Msagl.Core.DataStructures;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;

namespace Microsoft.Msagl.Routing {
    //this class contains a set of edge geometries, and set of node boundaries, ICurves, that might obstruct the edge routing 
    public class PreGraph {
        public List<EdgeGeometry> edgeGeometries;
        public Set<ICurve> nodeBoundaries;
        public Rectangle boundingBox;
       
        public PreGraph(EdgeGeometry[] egs, Set<ICurve> nodeBoundaries) {
            edgeGeometries = new List<EdgeGeometry>(egs);
            this.nodeBoundaries = new Set<ICurve>(nodeBoundaries);
            boundingBox = Rectangle.CreateAnEmptyBox();
            foreach (var curve in nodeBoundaries)
                boundingBox.Add(curve.BoundingBox);
        }

        public PreGraph() {
            edgeGeometries = new List<EdgeGeometry>();
            nodeBoundaries = new Set<ICurve>();
            boundingBox = Rectangle.CreateAnEmptyBox();
        }

        public void AddGraph(PreGraph a) {
            edgeGeometries.AddRange(a.edgeGeometries);
            nodeBoundaries += a.nodeBoundaries;
            boundingBox.Add(a.boundingBox);
        }

        public void AddNodeBoundary(ICurve curve) {
            nodeBoundaries.Insert(curve);
            boundingBox.Add(curve.BoundingBox);
        }
    }
}
