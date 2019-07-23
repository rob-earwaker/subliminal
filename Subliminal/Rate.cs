using System;

namespace Subliminal
{
    public class Rate
    {
        public Rate(long count, TimeSpan interval)
        {
            Count = count;
            Interval = interval;
            CountPerSecond = count / interval.TotalSeconds;
        }

        public long Count { get; }
        public TimeSpan Interval { get; }
        public double CountPerSecond { get; }
    }
}