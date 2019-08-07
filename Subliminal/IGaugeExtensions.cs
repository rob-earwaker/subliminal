using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public static class IGaugeExtensions
    {
        public static IGauge<Rate<int>> RateOfChange(this IGauge<int> gauge)
        {
            return gauge.RateOfChange(delta => delta.CurrentValue - delta.PreviousValue);
        }

        public static IGauge<Rate<ByteCount>> RateOfChange(this IGauge<ByteCount> gauge)
        {
            return gauge.RateOfChange(delta => delta.CurrentValue - delta.PreviousValue);
        }

        public static IGauge<Rate<Delta<TValue>>> RateOfChange<TValue>(this IGauge<TValue> gauge)
        {
            return gauge.RateOfChange(delta => delta);
        }

        public static IGauge<Rate<TDelta>> RateOfChange<TValue, TDelta>(
            this IGauge<TValue> gauge, Func<Delta<TValue>, TDelta> deltaSelector)
        {
            return gauge
                .TimeInterval()
                .Buffer(count: 2, skip: 1)
                .Select(buffer => new Rate<TDelta>(
                    delta: deltaSelector(new Delta<TValue>(previousValue: buffer[0].Value, currentValue: buffer[1].Value)),
                    interval: buffer[1].Interval))
                .AsGauge();
        }
    }
}
