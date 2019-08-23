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

        public double Fraction => TimeUsed.TotalMilliseconds / TimeAvailable.TotalMilliseconds;
        public double Percentage => Fraction * 100.0;
        public TimeSpan TimeAvailable => Interval;

        public double TotalFraction => Fraction / ProcessorCount;
        public double TotalPercentage => Percentage / ProcessorCount;
        public TimeSpan TotalTimeAvailable => TimeSpan.FromTicks(TimeAvailable.Ticks * ProcessorCount);
    }
}
