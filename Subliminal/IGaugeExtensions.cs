using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public static class IGaugeExtensions
    {
        public static IObservable<int> Delta(this IGauge<int> gauge)
        {
            return gauge.Delta(delta => delta.CurrentValue - delta.PreviousValue);
        }

        public static IObservable<long> Delta(this IGauge<long> gauge)
        {
            return gauge.Delta(delta => delta.CurrentValue - delta.PreviousValue);
        }

        public static IObservable<TimeSpan> Delta(this IGauge<TimeSpan> gauge)
        {
            return gauge.Delta(delta => delta.CurrentValue - delta.PreviousValue);
        }

        public static IObservable<Delta<TValue>> Delta<TValue>(this IGauge<TValue> gauge)
        {
            return gauge.Delta(delta => delta);
        }

        public static IObservable<TDelta> Delta<TValue, TDelta>(
            this IGauge<TValue> gauge, Func<Delta<TValue>, TDelta> deltaSelector)
        {
            return gauge
                .Buffer(count: 2, skip: 1)
                // If the observable completes, the last item will be emitted in a buffer containing just
                // that item. This will have already been emitted as the second value in the previous buffer,
                // so ignoring this smaller final buffer does not hide the final value from subscribers.
                .Where(buffer => buffer.Count == 2)
                .Select(buffer => deltaSelector(new Delta<TValue>(previousValue: buffer[0], currentValue: buffer[1])));
        }
    }
}
