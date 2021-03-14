using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public static class IGaugeExtensions
    {
        public static IObservable<double> Delta(this ILog<Measure<double>> log)
        {
            return log
                .Buffer(count: 2, skip: 1)
                // If the observable completes, the last value will be emitted in a buffer containing just
                // that value. A delta can not be calculated from a single value, so ignore it. The delta
                // between this final value and its predecessor will already have been emitted.
                .Where(buffer => buffer.Count == 2)
                .Select(buffer => buffer[1] - buffer[0]);
        }

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
