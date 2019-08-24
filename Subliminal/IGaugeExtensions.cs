using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public static class IGaugeExtensions
    {
        public static IObservable<int> Delta(this IGauge<int> gauge)
        {
            return gauge.Delta(value => value);
        }

        public static IObservable<int> Delta<TValue>(
            this IGauge<TValue> gauge, Func<TValue, int> valueSelector)
        {
            return gauge.Delta(valueSelector, delta => delta.CurrentValue - delta.PreviousValue);
        }

        public static IObservable<long> Delta(this IGauge<long> gauge)
        {
            return gauge.Delta(value => value);
        }

        public static IObservable<long> Delta<TValue>(
            this IGauge<TValue> gauge, Func<TValue, long> valueSelector)
        {
            return gauge.Delta(valueSelector, delta => delta.CurrentValue - delta.PreviousValue);
        }

        public static IObservable<TimeSpan> Delta(this IGauge<TimeSpan> gauge)
        {
            return gauge.Delta(value => value);
        }

        public static IObservable<TimeSpan> Delta<TValue>(
            this IGauge<TValue> gauge, Func<TValue, TimeSpan> valueSelector)
        {
            return gauge.Delta(valueSelector, delta => delta.CurrentValue - delta.PreviousValue);
        }

        public static IObservable<Delta<TValue>> Delta<TValue>(this IGauge<TValue> gauge)
        {
            return gauge.Delta(value => value);
        }

        public static IObservable<Delta<TNewValue>> Delta<TValue, TNewValue>(
            this IGauge<TValue> gauge, Func<TValue, TNewValue> valueSelector)
        {
            return gauge.Delta(valueSelector, delta => delta);
        }

        public static IObservable<TDelta> Delta<TValue, TNewValue, TDelta>(
            this IGauge<TValue> gauge, Func<TValue, TNewValue> valueSelector,
            Func<Delta<TNewValue>, TDelta> deltaSelector)
        {
            return gauge
                .Select(valueSelector)
                .Buffer(count: 2, skip: 1)
                // If the observable completes, the last item will be emitted in a buffer containing just
                // that item. This will have already been emitted as the second value in the previous buffer,
                // so ignoring this smaller final buffer does not hide the final value from subscribers.
                .Where(buffer => buffer.Count == 2)
                .Select(buffer =>
                    deltaSelector(new Delta<TNewValue>(previousValue: buffer[0], currentValue: buffer[1])));
        }

        public static IObservable<Rate<int>> RateOfChange(this IGauge<int> gauge)
        {
            return gauge.RateOfChange(delta => delta.CurrentValue - delta.PreviousValue);
        }

        public static IObservable<Rate<long>> RateOfChange(this IGauge<long> gauge)
        {
            return gauge.RateOfChange(delta => delta.CurrentValue - delta.PreviousValue);
        }

        public static IObservable<Rate<TimeSpan>> RateOfChange(this IGauge<TimeSpan> gauge)
        {
            return gauge.RateOfChange(delta => delta.CurrentValue - delta.PreviousValue);
        }

        public static IObservable<Rate<Delta<TValue>>> RateOfChange<TValue>(this IGauge<TValue> gauge)
        {
            return gauge.RateOfChange(delta => delta);
        }

        public static IObservable<Rate<TDelta>> RateOfChange<TValue, TDelta>(
            this IGauge<TValue> gauge, Func<Delta<TValue>, TDelta> deltaSelector)
        {
            return gauge
                .TimeInterval()
                .Buffer(count: 2, skip: 1)
                // If the observable completes, the last item will be emitted in a buffer containing just
                // that item. This will have already been emitted as the second value in the previous buffer,
                // so ignoring this smaller final buffer does not hide the final value from subscribers.
                .Where(buffer => buffer.Count == 2)
                .Select(buffer => new Rate<TDelta>(
                    delta: deltaSelector(new Delta<TValue>(previousValue: buffer[0].Value, currentValue: buffer[1].Value)),
                    interval: buffer[1].Interval));
        }
    }
}
