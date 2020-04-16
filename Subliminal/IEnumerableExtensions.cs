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
        /// Computes the average of a sequence of <see cref="Rate"/> values such that the
        /// resulting delta contains the sum of all deltas and intervals in the input sequence.
        /// </summary>
        public static Rate Average(this IEnumerable<Rate> rates)
        {
            return new Rate(
                delta: rates.Select(rate => rate.Delta).Sum(),
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
