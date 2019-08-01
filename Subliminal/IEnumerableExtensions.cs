using System;
using System.Collections.Generic;
using System.Linq;

namespace Subliminal
{
    public static class IEnumerableExtensions
    {
        public static TimeSpan Sum(this IEnumerable<TimeSpan> durations)
        {
            return durations.Aggregate(
                TimeSpan.Zero,
                (totalDuration, duration) => totalDuration + duration);
        }

        public static Rate<ByteCount> Average(this IEnumerable<Rate<ByteCount>> rates)
        {
            return new Rate<ByteCount>(
                delta: rates.Select(rate => rate.Delta).Sum(),
                interval: rates.Select(rate => rate.Interval).Sum());
        }

        public static BitRate Average(this IEnumerable<BitRate> bitRates)
        {
            return BitRate.Average(bitRates);
        }

        public static ByteCount Sum(this IEnumerable<ByteCount> byteCounts)
        {
            return ByteCount.Sum(byteCounts);
        }
    }
}
