using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Msagl.Core.Geometry;

using System.Linq;

namespace Microsoft.Msagl.Routing.Rectilinear.Nudging {
    /// <summary>
    /// represents a segment of a path
    /// </summary>
    public class LinkedPoint : IEnumerable<Point> {
        public Point Point { get; set; }

        public LinkedPoint Next { get; set; }

        public LinkedPoint(Point point) {
            Point = point;
        }

        public IEnumerator<Point> GetEnumerator() {
            for (var p = this; p != null; p = p.Next)
                yield return p.Point;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public double X { get { return Point.X; } }
 
        public double Y { get { return Point.Y; } }

        public void InsertVerts(int i, int j, Point[] points) {
            for (j--; i < j; j--)
                SetNewNext(points[j]);
        }

        public void InsertVertsInReverse(int i, int j, Point[] points) {
            for (i++; i < j; i++)
                SetNewNext(points[i]);
        }

        public void SetNewNext(Point p) {
            var nv = new LinkedPoint(p);
            var tmp = Next;
            Next = nv;
            nv.Next = tmp;            
            Debug.Assert( CompassVector.IsPureDirection(Point, Next.Point) );
        }

        public override string ToString() {
            return Point.ToString();
        }



    }
}
