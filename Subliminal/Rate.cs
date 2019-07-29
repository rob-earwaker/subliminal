using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;

namespace Subliminal
{
    public class Rate
    {
        public static readonly Rate Zero = new Rate(delta: 0.0, interval: TimeSpan.Zero);

        public Rate(double delta, TimeSpan interval)
        {
            Delta = delta;
            Interval = interval;
        }

        public double Delta { get; }
        public TimeSpan Interval { get; }
        public double DeltaPerSecond => Interval == TimeSpan.Zero ? 0.0 : Delta / Interval.TotalSeconds;
        public double DeltaPerMinute => Interval == TimeSpan.Zero ? 0.0 : Delta / Interval.TotalMinutes;
        public double DeltaPerHour => Interval == TimeSpan.Zero ? 0.0 : Delta / Interval.TotalHours;

        public static Rate FromTimeInterval(TimeInterval<long> delta)
        {
            return new Rate(delta.Value, delta.Interval);
        }

        public static Rate Average(IEnumerable<Rate> rates)
        {
            return rates.Aggregate(
                Rate.Zero,
                (aggregatedRate, rate) => new Rate(
                    aggregatedRate.Delta + rate.Delta,
                    aggregatedRate.Interval + rate.Interval));
        }
    }
}