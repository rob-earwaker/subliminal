using System;

namespace Subliminal
{
    public class ProcessorUsage
    {
        public ProcessorUsage(TimeSpan timeUsed, TimeSpan interval, int processorCount)
        {
            TimeUsed = timeUsed;
            Interval = interval;
            ProcessorCount = processorCount;
        }

        public TimeSpan TimeUsed { get; }
        public TimeSpan Interval { get; }
        public int ProcessorCount { get; }

        public double ProcessorFraction => TimeUsed.TotalMilliseconds / Interval.TotalMilliseconds;
        public double ProcessorPercentage => ProcessorFraction * 100.0;
        public double TotalFraction => ProcessorFraction / ProcessorCount;
        public double TotalPercentage => TotalFraction * 100.0;
        public TimeSpan TotalTimeAvailable => TimeSpan.FromMilliseconds(ProcessorCount * TimeUsed.TotalMilliseconds);
    }
}
