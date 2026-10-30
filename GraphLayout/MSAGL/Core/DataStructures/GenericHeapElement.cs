namespace Microsoft.Msagl.Core.DataStructures {
    public class GenericHeapElement<T> {
        public int indexToA;
        public double priority;
        public T v;//value
        public GenericHeapElement(int index, double priority, T v) {
            indexToA = index;
            this.priority = priority;
            this.v = v;
        }
    }
}