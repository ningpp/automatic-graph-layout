﻿using Microsoft.Msagl.Core.Geometry;

namespace Microsoft.Msagl.Core.Layout
{
    public class RectangleObstacle : IObstacle
    {
        public RectangleObstacle(Rectangle r)
        {
            this.Rectangle = r;
        }

        public RectangleObstacle(Rectangle r, object data)
        {
            this.Rectangle = r;
            this.Data = data;
        }

        public Rectangle Rectangle
        {
            get;
            private set;
        }

        public object Data
        {
            get;
            private set;
        }
    }
}