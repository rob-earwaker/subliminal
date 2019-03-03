using System;

namespace Subliminal
{
    public class ProcessorUsage
    {
        public ProcessorUsage(TimeSpan processorTime, TimeSpan interval, int processorCount)
        {
            ProcessorTime = processorTime;
            Interval = interval;
            ProcessorCount = processorCount;
        }

        public TimeSpan ProcessorTime { get; }
        public TimeSpan Interval { get; }
        public int ProcessorCount { get; }
        public double Fraction => ProcessorTime.TotalMilliseconds / Interval.TotalMilliseconds;
        public double Percentage => Fraction * 100;
    }
}
