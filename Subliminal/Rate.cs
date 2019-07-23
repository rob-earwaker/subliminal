using System;

namespace Subliminal
{
    public class Rate
    {
        public Rate(long count, TimeSpan interval)
        {
            Count = count;
            Interval = interval;
        }

        public long Count { get; }
        public TimeSpan Interval { get; }
        public double CountPerSecond => Count / Interval.TotalSeconds;
    }
}