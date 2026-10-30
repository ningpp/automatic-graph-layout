using System.Collections.Generic;
using System.Linq;
using Microsoft.Msagl.Core;
using Microsoft.Msagl.Core.GraphAlgorithms;
using Microsoft.Msagl.Core.Layout;

namespace Microsoft.Msagl.Layout.Layered {
    public class NetworkSimplexForGeneralGraph : LayerCalculator {
        BasicGraphOnEdges<PolyIntEdge> graph;
        /// <summary>
        /// a place holder for the cancel flag
        /// </summary>
        public CancelToken Cancel { get; set; }

        public int[] GetLayers() {
            NetworkSimplex ns = new NetworkSimplex(graph, this.Cancel);
            return ns.GetLayers();
        }

        
        public NetworkSimplexForGeneralGraph(BasicGraph<Node, PolyIntEdge> graph, CancelToken cancelObject) {
            this.graph = graph;
            this.Cancel = cancelObject;
        }

    }
}
