using System;
using System.Collections.Generic;
using System.Linq;

namespace Subliminal
{
    /// <summary>
    /// Contains extensions for the <see cref="IEnumerable{T}" /> interface.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Computes the average of a sequence of <see cref="TimeSpan"/> values.
        /// </summary>
        public static TimeSpan Average(this IEnumerable<TimeSpan> durations)
        {
            return TimeSpan.FromMilliseconds(durations.Average(duration => duration.TotalMilliseconds));
        }

        /// <summary>
        /// Computes the average of a sequence of <see cref="TimeSpan"/> values that are
        /// obtained by invoking a transform function on each element of the input sequence.
        /// </summary>
        public static TimeSpan Average<TSource>(
            this IEnumerable<TSource> source, Func<TSource, TimeSpan> durationSelector)
        {
            return source.Select(durationSelector).Average();
        }

        /// <summary>
        /// Computes the average of a sequence of <see cref="Rate{TDelta}"/> values such that the
        /// resulting delta contains the sum of all deltas and intervals in the input sequence.
        /// </summary>
        public static Rate<int> Average(this IEnumerable<Rate<int>> rates)
        {
            return rates.Average(deltas => deltas.Sum());
        }

        /// <summary>
        /// Computes the average of a sequence of <see cref="Rate{TDelta}"/> values such that the
        /// resulting delta contains the sum of all deltas and intervals in the input sequence.
        /// </summary>
        public static Rate<long> Average(this IEnumerable<Rate<long>> rates)
        {
            return rates.Average(deltas => deltas.Sum());
        }

        /// <summary>
        /// Computes the average of a sequence of <see cref="Rate{TDelta}"/> values such that the
        /// resulting delta contains the sum of all deltas and intervals in the input sequence.
        /// </summary>
        public static Rate<double> Average(this IEnumerable<Rate<double>> rates)
        {
            return rates.Average(deltas => deltas.Sum());
        }

        /// <summary>
        /// Computes the average of a sequence of <see cref="Rate{TDelta}"/> values such
        /// that the resulting delta contains the value of all deltas and the sum of all
        /// intervals in the input sequence.
        /// </summary>
        public static Rate<IList<TDelta>> Average<TDelta>(this IEnumerable<Rate<TDelta>> rates)
        {
            return rates.Average<TDelta, IList<TDelta>>(deltas => deltas.ToList());
        }

        /// <summary>
        /// Computes the average of a sequence of <see cref="Rate{TDelta}"/> values such
        /// that the resulting delta contains the transformed value of all deltas and the
        /// sum of all intervals in the input sequence.
        /// </summary>
        public static Rate<TDeltaSum> Average<TDelta, TDeltaSum>(
            this IEnumerable<Rate<TDelta>> rates, Func<IEnumerable<TDelta>, TDeltaSum> sumDeltas)
        {
            return new Rate<TDeltaSum>(
                delta: sumDeltas(rates.Select(rate => rate.Delta)),
                interval: rates.Select(rate => rate.Interval).Sum());
        }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="TimeSpan"/> values.
        /// </summary>
        public static TimeSpan Sum(this IEnumerable<TimeSpan> durations)
        {
            return durations.Aggregate(TimeSpan.Zero, (total, duration) => total + duration);
        }
    }
}
