using System;
using System.Collections.Generic;
using System.Linq;

namespace Subliminal
{
    public static class Summary
    {
        public static Summary<double> Summarize(IEnumerable<double> values)
        {
            if (!values.Any())
                throw new ArgumentException("No values to summarize", nameof(values));

            var orderedValues = values.OrderBy(value => value).ToArray();

            var min = orderedValues.First();
            var max = orderedValues.Last();

            var median = orderedValues.Length % 2 == 1
                ? orderedValues[orderedValues.Length / 2]
                : 0.5 * (orderedValues[orderedValues.Length / 2 - 1] + orderedValues[orderedValues.Length / 2]);

            var mean = orderedValues.Average();

            var variance = orderedValues
                .Select(value => Math.Pow(value - mean, 2))
                .Average();

            var std = Math.Sqrt(variance);

            return new Summary<double>(min, max, median, mean, std);
        }

        public static Summary<TimeSpan> Summarize(IEnumerable<TimeSpan> durations)
        {
            var millisecondSummary = Summarize(durations.Select(duration => duration.TotalMilliseconds));

            return new Summary<TimeSpan>(
                TimeSpan.FromMilliseconds(millisecondSummary.Min),
                TimeSpan.FromMilliseconds(millisecondSummary.Max),
                TimeSpan.FromMilliseconds(millisecondSummary.Median),
                TimeSpan.FromMilliseconds(millisecondSummary.Mean),
                TimeSpan.FromMilliseconds(millisecondSummary.Std));
        }
    }

    public class Summary<TValue>
    {
        public Summary(TValue min, TValue max, TValue median, TValue mean, TValue std)
        {
            Min = min;
            Max = max;
            Median = median;
            Mean = mean;
            Std = std;
        }

        public TValue Min { get; }
        public TValue Max { get; }
        public TValue Median { get; }
        public TValue Mean { get; }
        public TValue Std { get; }
    }
}
