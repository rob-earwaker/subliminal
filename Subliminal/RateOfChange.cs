using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;

namespace Subliminal
{
    public class RateOfChange
    {
        public static readonly RateOfChange Zero = new RateOfChange(delta: 0.0, interval: TimeSpan.Zero);

        public RateOfChange(double delta, TimeSpan interval)
        {
            Delta = delta;
            Interval = interval;
        }

        public double Delta { get; }
        public TimeSpan Interval { get; }
        public double DeltaPerSecond => Interval == TimeSpan.Zero ? 0.0 : Delta / Interval.TotalSeconds;
        public double DeltaPerMinute => Interval == TimeSpan.Zero ? 0.0 : Delta / Interval.TotalMinutes;
        public double DeltaPerHour => Interval == TimeSpan.Zero ? 0.0 : Delta / Interval.TotalHours;

        public static RateOfChange FromTimeInterval(TimeInterval<long> delta)
        {
            return new RateOfChange(delta.Value, delta.Interval);
        }

        public static RateOfChange Average(IEnumerable<RateOfChange> ratesOfChange)
        {
            return ratesOfChange.Aggregate(
                RateOfChange.Zero,
                (aggregatedRateOfChange, rateOfChange) => new RateOfChange(
                    aggregatedRateOfChange.Delta + rateOfChange.Delta,
                    aggregatedRateOfChange.Interval + rateOfChange.Interval));
        }
    }
}