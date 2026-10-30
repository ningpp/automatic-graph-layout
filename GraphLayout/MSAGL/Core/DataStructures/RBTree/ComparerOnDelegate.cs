using System;
using System.Collections.Generic;

namespace Microsoft.Msagl.Core.DataStructures {
    public class ComparerOnDelegate<T> : IComparer<T> {
        readonly Func<T, T, int> comparer;

        public ComparerOnDelegate(Func<T, T, int> compare) {
            comparer= compare;
        }
     
        public int Compare(T x, T y) {
            return comparer(x, y);
        }
    }
}