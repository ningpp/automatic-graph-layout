using System.Collections.Generic;
using Microsoft.Msagl.Core.DataStructures;

namespace Microsoft.Msagl.Core.GraphAlgorithms {
    public class NodeInfo {
        Set<int> outEdges = new Set<int>();

        public Set<int> OutEdges {
            get { return outEdges; }
        }
        Set<int> inEdges = new Set<int>();

        public Set<int> InEdges {
            get { return inEdges; }
        }
        Set<int> outConstrainedEdges = new Set<int>();

        public Set<int> OutConstrainedEdges {
            get { return outConstrainedEdges; }
        }

        Set<int> inConstrainedEdges = new Set<int>();

        public Set<int> InConstrainedEdges {
            get { return inConstrainedEdges; }
        }
        /// <summary>
        /// it is the out degree without the in degree
        /// </summary>
        public int DeltaDegree {
            get { return InDegree-OutDegree; }
        }
        public void AddOutEdge(int v) {
            outEdges.Insert(v);
        }
        public void RemoveOutEdge(int v) {
            outEdges.Remove(v);
        }

        public void AddInEdge(int v) {
            inEdges.Insert(v);
        }
        public void RemoveInEdge(int v) {
            inEdges.Remove(v);
        }
        public void AddOutConstrainedEdge(int v) {
            outConstrainedEdges.Insert(v);
        }
        public void RemoveOutConstrainedEdge(int v) {
            outConstrainedEdges.Remove(v);
        }

        public void AddInConstrainedEdge(int v) {
            inConstrainedEdges.Insert(v);
        }
        public void RemoveInConstrainedEdge(int v) {
            inConstrainedEdges.Remove(v);
        }

        public int OutDegree {
            get {
                return outEdges.Count+outConstrainedEdges.Count;
            }
        }
        public int InDegreeOfConstrainedEdges {
            get {
                return inConstrainedEdges.Count;
            }
        }
        public int InDegree {
            get { return inEdges.Count+inConstrainedEdges.Count; }
        }


        /// <summary>
        /// including constrained neighbors
        /// </summary>
        public IEnumerable<int> AllNeighbors {
            get {
                foreach (int v in this.OutConstrainedEdges)
                    yield return v;
                foreach (int v in this.InConstrainedEdges)
                    yield return v;
                foreach (int v in this.OutEdges)
                    yield return v;
                foreach (int v in this.InEdges)
                    yield return v;
            }
        }
    }
}
