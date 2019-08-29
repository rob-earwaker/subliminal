using System;
using System.Reactive.Linq;

namespace Subliminal
{
    /// <summary>
    /// Contains extensions for the <see cref="IGauge{TValue}" /> interface.
    /// </summary>
    public static class IGaugeExtensions
    {
        /// <summary>
        /// Measures the delta between values emitted from a gauge by calculating the difference
        /// between each value and the previous one in the sequence.
        /// </summary>
        public static IObservable<int> Delta(this IGauge<int> gauge)
        {
            return gauge.Delta(value => value);
        }

        /// <summary>
        /// Measures the delta between transformed values emitted from a gauge by calculating
        /// the difference between each transformed value and the previous one in the sequence.
        /// </summary>
        public static IObservable<int> Delta<TValue>(
            this IGauge<TValue> gauge, Func<TValue, int> valueSelector)
        {
            return gauge.Delta(valueSelector, delta => delta.CurrentValue - delta.PreviousValue);
        }

        /// <summary>
        /// Measures the delta between values emitted from a gauge by calculating the difference
        /// between each value and the previous one in the sequence.
        /// </summary>
        public static IObservable<long> Delta(this IGauge<long> gauge)
        {
            return gauge.Delta(value => value);
        }

        /// <summary>
        /// Measures the delta between transformed values emitted from a gauge by calculating
        /// the difference between each transformed value and the previous one in the sequence.
        /// </summary>
        public static IObservable<long> Delta<TValue>(
            this IGauge<TValue> gauge, Func<TValue, long> valueSelector)
        {
            return gauge.Delta(valueSelector, delta => delta.CurrentValue - delta.PreviousValue);
        }

        /// <summary>
        /// Measures the delta between values emitted from a gauge by calculating the difference
        /// between each value and the previous one in the sequence.
        /// </summary>
        public static IObservable<TimeSpan> Delta(this IGauge<TimeSpan> gauge)
        {
            return gauge.Delta(value => value);
        }

        /// <summary>
        /// Measures the delta between transformed values emitted from a gauge by calculating
        /// the difference between each transformed value and the previous one in the sequence.
        /// </summary>
        public static IObservable<TimeSpan> Delta<TValue>(
            this IGauge<TValue> gauge, Func<TValue, TimeSpan> valueSelector)
        {
            return gauge.Delta(valueSelector, delta => delta.CurrentValue - delta.PreviousValue);
        }

        /// <summary>
        /// Measures the delta between values emitted from a gauge by combining each
        /// value with the previous one in the sequence.
        /// </summary>
        public static IObservable<Delta<TValue>> Delta<TValue>(this IGauge<TValue> gauge)
        {
            return gauge.Delta(value => value);
        }

        /// <summary>
        /// Measures the delta between transformed values emitted from a gauge by combining each
        /// transformed value with the previous one in the sequence.
        /// </summary>
        public static IObservable<Delta<TNewValue>> Delta<TValue, TNewValue>(
            this IGauge<TValue> gauge, Func<TValue, TNewValue> valueSelector)
        {
            return gauge.Delta(valueSelector, delta => delta);
        }

        /// <summary>
        /// Measures the delta between transformed values emitted from a gauge by combining each
        /// transformed value with the previous one in the sequence.
        /// </summary>
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
                .Select(buffer => deltaSelector(
                    new Delta<TNewValue>(previousValue: buffer[0], currentValue: buffer[1])));
        }

        /// <summary>
        /// Measures the rate of change between values emitted from a gauge by combining the difference
        /// between each value and the previous one in the sequence with the time interval between them.
        /// </summary>
        public static IObservable<Rate<int>> RateOfChange(this IGauge<int> gauge)
        {
            return gauge.RateOfChange(value => value);
        }

        /// <summary>
        /// Measures the rate of change between transformed values emitted from a gauge by combining the difference
        /// between each transformed value and the previous one in the sequence with the time interval between them.
        /// </summary>
        public static IObservable<Rate<int>> RateOfChange<TValue>(
            this IGauge<TValue> gauge, Func<TValue, int> valueSelector)
        {
            return gauge.RateOfChange(valueSelector, delta => delta.CurrentValue - delta.PreviousValue);
        }

        /// <summary>
        /// Measures the rate of change between values emitted from a gauge by combining the difference
        /// between each value and the previous one in the sequence with the time interval between them.
        /// </summary>
        public static IObservable<Rate<long>> RateOfChange(this IGauge<long> gauge)
        {
            return gauge.RateOfChange(value => value);
        }

        /// <summary>
        /// Measures the rate of change between transformed values emitted from a gauge by combining the difference
        /// between each transformed value and the previous one in the sequence with the time interval between them.
        /// </summary>
        public static IObservable<Rate<long>> RateOfChange<TValue>(
            this IGauge<TValue> gauge, Func<TValue, long> valueSelector)
        {
            return gauge.RateOfChange(valueSelector, delta => delta.CurrentValue - delta.PreviousValue);
        }

        /// <summary>
        /// Measures the rate of change between values emitted from a gauge by combining the difference
        /// between each value and the previous one in the sequence with the time interval between them.
        /// </summary>
        public static IObservable<Rate<TimeSpan>> RateOfChange(this IGauge<TimeSpan> gauge)
        {
            return gauge.RateOfChange(value => value);
        }

        /// <summary>
        /// Measures the rate of change between transformed values emitted from a gauge by combining the difference
        /// between each transformed value and the previous one in the sequence with the time interval between them.
        /// </summary>
        public static IObservable<Rate<TimeSpan>> RateOfChange<TValue>(
            this IGauge<TValue> gauge, Func<TValue, TimeSpan> valueSelector)
        {
            return gauge.RateOfChange(valueSelector, delta => delta.CurrentValue - delta.PreviousValue);
        }

        /// <summary>
        /// Measures the rate of change between values emitted from a gauge by combining each value
        /// with the previous one in the sequence along with the time interval between them.
        /// </summary>
        public static IObservable<Rate<Delta<TValue>>> RateOfChange<TValue>(this IGauge<TValue> gauge)
        {
            return gauge.RateOfChange(value => value);
        }

        /// <summary>
        /// Measures the rate of change between transformed values emitted from a gauge by combining each 
        /// transformed value with the previous one in the sequence along with the time interval between them.
        /// </summary>
        public static IObservable<Rate<Delta<TNewValue>>> RateOfChange<TValue, TNewValue>(
            this IGauge<TValue> gauge, Func<TValue, TNewValue> valueSelector)
        {
            return gauge.RateOfChange(valueSelector, delta => delta);
        }

        /// <summary>
        /// Measures the rate of change between transformed values emitted from a gauge by combining each 
        /// transformed value with the previous one in the sequence along with the time interval between them.
        /// </summary>
        public static IObservable<Rate<TDelta>> RateOfChange<TValue, TNewValue, TDelta>(
            this IGauge<TValue> gauge, Func<TValue, TNewValue> valueSelector,
            Func<Delta<TNewValue>, TDelta> deltaSelector)
        {
            return gauge
                .Select(valueSelector)
                .TimeInterval()
                .Buffer(count: 2, skip: 1)
                // If the observable completes, the last item will be emitted in a buffer containing just
                // that item. This will have already been emitted as the second value in the previous buffer,
                // so ignoring this smaller final buffer does not hide the final value from subscribers.
                .Where(buffer => buffer.Count == 2)
                .Select(buffer => new Rate<TDelta>(
                    delta: deltaSelector(
                        new Delta<TNewValue>(previousValue: buffer[0].Value, currentValue: buffer[1].Value)),
                    interval: buffer[1].Interval));
        }
    }
}
