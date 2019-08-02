using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public static class IGaugeExtensions
    {
        public static IGauge<Rate<int>> Rate(this IGauge<int> gauge)
        {
            return gauge.Rate(delta => delta.CurrentValue - delta.PreviousValue);
        }

        public static IGauge<Rate<ByteCount>> Rate(this IGauge<ByteCount> gauge)
        {
            return gauge.Rate(delta => delta.CurrentValue - delta.PreviousValue);
        }

        public static IGauge<Rate<Delta<TValue>>> Rate<TValue>(this IGauge<TValue> gauge)
        {
            return gauge.Rate(delta => delta);
        }

        public static IGauge<Rate<TDelta>> Rate<TValue, TDelta>(
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
