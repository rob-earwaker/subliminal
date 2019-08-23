using System;
using System.Collections.Generic;
using System.Linq;

namespace Subliminal
{
    public static class IEnumerableExtensions
    {
        public static TimeSpan Average(this IEnumerable<TimeSpan> durations)
        {
            return TimeSpan.FromMilliseconds(durations.Average(duration => duration.TotalMilliseconds));
        }

        public static TimeSpan Average<TSource>(
            this IEnumerable<TSource> source, Func<TSource, TimeSpan> durationSelector)
        {
            return source.Select(durationSelector).Average();
        }

        public static Rate<int> Average(this IEnumerable<Rate<int>> rates)
        {
            return rates.Average(deltas => deltas.Sum());
        }

        public static Rate<long> Average(this IEnumerable<Rate<long>> rates)
        {
            return rates.Average(deltas => deltas.Sum());
        }

        public static Rate<IList<TDelta>> Average<TDelta>(this IEnumerable<Rate<TDelta>> rates)
        {
            return rates.Average<TDelta, IList<TDelta>>(deltas => deltas.ToList());
        }

        public static Rate<TDeltaSum> Average<TDelta, TDeltaSum>(
            this IEnumerable<Rate<TDelta>> rates, Func<IEnumerable<TDelta>, TDeltaSum> sumDeltas)
        {
            return new Rate<TDeltaSum>(
                delta: sumDeltas(rates.Select(rate => rate.Delta)),
                interval: rates.Select(rate => rate.Interval).Sum());
        }

        public static TimeSpan Sum(this IEnumerable<TimeSpan> durations)
        {
            return durations.Aggregate(TimeSpan.Zero, (total, duration) => total + duration);
        }
    }
}
