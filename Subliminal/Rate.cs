using System;
using System.Collections.Generic;
using System.Linq;

namespace Subliminal
{
    public class Rate
    {
        public static readonly Rate Zero = new Rate(delta: 0.0, interval: TimeSpan.MaxValue);

        public Rate(double delta, TimeSpan interval)
        {
            Delta = delta;
            Interval = interval;
        }

        public double Delta { get; }
        public TimeSpan Interval { get; }

        public double DeltaPerMillisecond => Interval == TimeSpan.Zero ? 0.0 : Delta / Interval.TotalMilliseconds;
        public double DeltaPerSecond => Interval == TimeSpan.Zero ? 0.0 : Delta / Interval.TotalSeconds;
        public double DeltaPerMinute => Interval == TimeSpan.Zero ? 0.0 : Delta / Interval.TotalMinutes;
        public double DeltaPerHour => Interval == TimeSpan.Zero ? 0.0 : Delta / Interval.TotalHours;
        public double DeltaPerDay => Interval == TimeSpan.Zero ? 0.0 : Delta / Interval.TotalDays;

        public static Rate Average(IEnumerable<Rate> rates)
        {
            return rates.Aggregate(
                Rate.Zero,
                (averageRate, rate) => new Rate(
                    averageRate.Delta + rate.Delta,
                    averageRate.Interval + rate.Interval));
        }
    }
}