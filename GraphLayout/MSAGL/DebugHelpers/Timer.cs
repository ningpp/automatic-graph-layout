using System.Diagnostics;

namespace Microsoft.Msagl.DebugHelpers {
    /// <summary>
    /// High-resolution timer using System.Diagnostics.Stopwatch
    /// </summary>
    public class Timer {
        private long startTime;
        private long stopTime;
        private readonly long freq;
        /// <summary>
        /// ctor
        /// </summary>
        public Timer() {
            freq = Stopwatch.Frequency;
        }
        /// <summary>
        /// Start the timer
        /// </summary>
        /// <returns>long - tick count</returns>
        public long Start() {
            startTime = Stopwatch.GetTimestamp();
            return startTime;
        }
        /// <summary>
        /// Stop timer 
        /// </summary>
        /// <returns>long - tick count</returns>
        public long Stop() {
            stopTime = Stopwatch.GetTimestamp();
            return stopTime;
        }
        /// <summary>
        /// Return the duration of the timer in seconds.
        /// </summary>
        /// <returns>double - duration</returns>
        public double Duration {
            get {
                return (double)(stopTime - startTime) / (double)freq;
            }
        }
        /// <summary>
        /// Frequency of timer (no counts in one second on this machine)
        /// </summary>
        ///<returns>long - Frequency</returns>
        public long Frequency {
            get {
                return freq;
            }
        }
    }

}




