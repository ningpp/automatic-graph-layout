using System;

namespace Microsoft.Msagl.Core.Geometry.Curves {
    /// <summary>
    /// Computes the closest point on a curve to a given point.
    /// </summary>
    public class ClosestPointOnCurve {
        ClosestPointOnCurve() { }
        /// <summary>
        /// Gets the parameter of the closest point on the curve to the given point, using the hint as the initial guess.
        /// </summary>
        /// <param name="curve">The curve.</param>
        /// <param name="a">The target point.</param>
        /// <param name="hint">The initial parameter guess.</param>
        /// <param name="low">The lower bound of the parameter.</param>
        /// <param name="high">The upper bound of the parameter.</param>
        public static double ClosestPoint(ICurve curve, Point a, double hint, double low, double high) {
            const int numberOfIterationsMax = 5;
            const int numberOfOverShootsMax = 5;
            double t = hint;
      
            int numberOfIteration = 0;
            int numberOfOvershoots = 0;
            double dt;
            bool abort = false;
            do {
                Point c = curve[t];
                Point ct = curve.Derivative(t);
                Point ctt = curve.SecondDerivative(t);

                double secondDerivative = ct * ct + (c - a) * ctt;

                if (Math.Abs(secondDerivative) < ApproximateComparer.Tolerance) 
                    return t;

                dt = (c - a) * ct / secondDerivative;
               
                t -= dt;
                
                if (t > high + ApproximateComparer.Tolerance) {
                    t = high;
                    numberOfOvershoots++;
                } else if (t < low - ApproximateComparer.Tolerance) {
                    t = low;
                    numberOfOvershoots++;
                }
                numberOfIteration++;
            } while (Math.Abs(dt) > ApproximateComparer.Tolerance &&!
                (abort = (numberOfIteration >= numberOfIterationsMax || numberOfOvershoots >= numberOfOverShootsMax)));

            //may be the initial value was just fine
            if (abort && (curve[hint] - a).Length < ApproximateComparer.DistanceEpsilon) 
                t = hint;

            return ClosestOfTAndIntervalEnds(curve, a, t, low, high);

           }

        static double ClosestOfTAndIntervalEnds(ICurve curve, Point a, double t, double low, double high) {
            double bestParam = t;
            double bestSquaredDistance = SquaredDistance(curve, a, t);
            double squaredDistanceAtLow = SquaredDistance(curve, a, low);
            if (squaredDistanceAtLow < bestSquaredDistance) {
                bestParam = low;
                bestSquaredDistance = squaredDistanceAtLow;
            }
            double squaredDistanceAtHigh = SquaredDistance(curve, a, high);
            if (squaredDistanceAtHigh < bestSquaredDistance)
                bestParam = high;
            return bestParam;
        }

        static double SquaredDistance(ICurve curve, Point a, double t) {
            Point delta = curve[t] - a;
            return delta*delta;
        }

    }
}
