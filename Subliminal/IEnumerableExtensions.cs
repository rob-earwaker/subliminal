using System;
using System.Collections.Generic;
using System.Linq;

namespace Subliminal
{
    public static class IEnumerableExtensions
    {
        public static Rate<int> Average(this IEnumerable<Rate<int>> rates)
        {
            return rates.Average(deltas => deltas.Sum());
        }

        public static Rate<ByteCount> Average(this IEnumerable<Rate<ByteCount>> rates)
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

        public static ByteCount Sum(this IEnumerable<ByteCount> byteCounts)
        {
            return byteCounts.Aggregate(ByteCount.Zero, (total, byteCount) => total + byteCount);
        }
    }
}
