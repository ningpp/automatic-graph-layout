using System;

namespace Microsoft.Msagl.Core.DataStructures {
    /// <summary>
    /// this class behaves like one dimensional bounding box
    /// </summary>
    public class RealNumberSpan{
        /// <summary>
        /// Constructs an empty span.
        /// </summary>
        public RealNumberSpan(){
            IsEmpty = true;
        }

        /// <summary>
        /// True if no values have been added to the span.
        /// </summary>
        public bool IsEmpty { get; set; }

        /// <summary>
        /// Adds a value to the span, extending the minimum or maximum if needed.
        /// </summary>
        public void AddValue(double x){
            if(IsEmpty){
                Min = Max = x;
                IsEmpty = false;
            } else if(x < Min)
                Min = x;
            else if(x > Max)
                Max = x;
        }

        /// <summary>
        /// The minimum value of the span.
        /// </summary>
        public double Min { get; set; }
        /// <summary>
        /// The maximum value of the span.
        /// </summary>
        public double Max { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double Length{
            get { return Max-Min; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)")]
        public override string ToString() {
            return IsEmpty ? "empty" : String.Format("{0},{1}", Min, Max);
        }

    }
}