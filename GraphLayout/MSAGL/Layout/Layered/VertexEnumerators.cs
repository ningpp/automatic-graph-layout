using System;
using System.Collections.Generic;
using System.Collections;

using Microsoft.Msagl.Core.GraphAlgorithms;

namespace Microsoft.Msagl.Layout.Layered {
    /// <summary>
    /// Enumerator of the vertex successors
    /// </summary>
    public class SuccEnumerator : IEnumerator<int> {
        IEnumerator edges;

        public void Reset() {
            edges.Reset();
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
        }


        public bool MoveNext() {
            return edges.MoveNext();
        }

        public SuccEnumerator(IEnumerator edges) {
            this.edges = edges;
        }

        #region IEnumerator<int> Members

        public int Current {
            get {
                return((PolyIntEdge)edges.Current).Target;
            }

        }

        #endregion

        #region IEnumerator Members

        object IEnumerator.Current {
            get {
                throw new NotImplementedException();
            }
        }

        #endregion
    }

    /// <summary>
    /// Enumeration of the vertex predecessors
    /// </summary>
    public class PredEnumerator : IEnumerator<int> {
        IEnumerator edges;

        public void Dispose() { GC.SuppressFinalize(this); }

        public void Reset() {
            edges.Reset();
        }

#if SHARPKIT //http://code.google.com/p/sharpkit/issues/detail?id=203
        public int Current {
#else
        int IEnumerator<int>.Current {
#endif
            get {
                return ((PolyIntEdge)edges.Current).Source;
            }
        }

        object IEnumerator.Current {
            get {
                PolyIntEdge l = edges.Current as PolyIntEdge;
                return l.Source;
            }
        }

        public bool MoveNext() {
            return edges.MoveNext();
        }

        public PredEnumerator(IEnumerator edges) {
            this.edges = edges;
        }
    }

    public class EmptyEnumerator : IEnumerator<int> {
        public bool MoveNext() { return false; }

#if SHARPKIT //http://code.google.com/p/sharpkit/issues/detail?id=203
        public int Current { get { return 0; } }
#else
        int IEnumerator<int>.Current { get { return 0; } }
#endif
        object IEnumerator.Current { get { return 0; } }

        void IEnumerator.Reset() { }


        public void Dispose() { GC.SuppressFinalize(this); }

    }

    public class Pred : IEnumerable<int> {
        #region IEnumerable Members

        BasicGraphOnEdges<PolyIntEdge> graph;

        int vert;

#if SHARPKIT //http://code.google.com/p/sharpkit/issues/detail?id=203
        public IEnumerator<int> GetEnumerator() {
#else
        IEnumerator<int> IEnumerable<int>.GetEnumerator() {
#endif
        IEnumerable e = graph.InEdges(vert);
            if (e == null) {
                return new EmptyEnumerator();
            } else
                return new PredEnumerator(e.GetEnumerator());

        }

        IEnumerator IEnumerable.GetEnumerator() {
            IEnumerable e = graph.InEdges(vert);
            if (e == null) {
                return new EmptyEnumerator();
            } else
                return new PredEnumerator(e.GetEnumerator());

        }

        public Pred(BasicGraphOnEdges<PolyIntEdge> g, int v) {
            this.graph = g;
            this.vert = v;
        }

        #endregion


    }


    public class Succ {
        #region IEnumerable Members

        BasicGraphOnEdges<PolyIntEdge> graph;

        int vert;

        public IEnumerator<int> GetEnumerator() {
            return new SuccEnumerator(graph.OutEdges(vert).GetEnumerator());
        }

        public Succ(BasicGraphOnEdges<PolyIntEdge> g, int v) {
            this.graph = g;
            this.vert = v;
        }



        #endregion
    }

}
