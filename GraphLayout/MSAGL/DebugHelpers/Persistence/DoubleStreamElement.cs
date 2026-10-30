﻿namespace Microsoft.Msagl.DebugHelpers {
    public class DoubleStreamElement : CurveStreamElement {
        public DoubleStreamElement(double res) {
            Value = res;
        }

        public double Double { get { return (double)Value; } }
    }
}