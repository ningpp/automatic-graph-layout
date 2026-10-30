using System.Diagnostics;
using Microsoft.Msagl.Core.Geometry;

namespace Microsoft.Msagl.Routing.Rectilinear.Nudging
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class SegWithIndex {
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
        public Point[] Points;
        public int I;//offset
    
        public SegWithIndex(Point[] pts, int i) {
            Debug.Assert(i<pts.Length&&i>=0);
            Points = pts;
            I = i;
        }

        public Point Start {get{return Points[I];}}
        public Point End{ get{return Points[I+1];}}
    
        override public bool Equals(object obj) {
            var other = (SegWithIndex) obj;
            return other.Points== Points&& other.I == I;
        }

        

    }
}