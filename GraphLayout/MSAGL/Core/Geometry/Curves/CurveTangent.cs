
namespace Microsoft.Msagl.Core.Geometry.Curves {
    /// <summary>
    /// used to calculate the around curve 
    /// </summary>
    public class CurveTangent {
        public Point touchPoint;

        public Point direction;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="touch"></param>
        /// <param name="direction"></param>
        public CurveTangent(Point touch, Point direction) {
            this.touchPoint = touch;
            this.direction = direction;
        }
    }
}
