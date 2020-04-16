using System;
using System.Reactive.Linq;

namespace Subliminal
{
    /// <summary>
    /// Contains extensions for the <see cref="IGauge" /> interface.
    /// </summary>
    public static class IGaugeExtensions
    {
        /// <summary>
        /// Measures the delta between values emitted from a gauge by calculating the difference
        /// between each value and the previous one in the sequence.
        /// </summary>
        public static IObservable<double> Delta(this IGauge gauge)
        {
            return gauge
                .Buffer(count: 2, skip: 1)
                // If the observable completes, the last value will be emitted in a buffer containing just
                // that value. A delta can not be calculated from a single value, so ignore it. The delta
                // between this final value and its predecessor will already have been emitted.
                .Where(buffer => buffer.Count == 2)
                .Select(buffer => buffer[1] - buffer[0]);
        }

        /// <summary>
        /// Measures the rate of change between values emitted from a gauge by combining the difference
        /// between each value and the previous one in the sequence with the time interval between them.
        /// </summary>
        public static IObservable<Rate> RateOfChange(this IGauge gauge)
        {
            return gauge
                // Time interval must be determined before buffering otherwise the interval of the first
                // rate value will be the time since the start of the subscription rather than the time
                // since the first value.
                .TimeInterval()
                .Buffer(count: 2, skip: 1)
                // If the observable completes, the last value will be emitted in a buffer containing just
                // that value. A delta can not be calculated from a single value, so ignore it. The delta
                // between this final value and its predecessor will already have been emitted.
                .Where(buffer => buffer.Count == 2)
                .Select(buffer => new Rate(
                    delta: buffer[1].Value - buffer[0].Value,
                    interval: buffer[1].Interval));
        }
    }
}
