using System;

namespace Subliminal
{
    public class ProcessorUsage
    {
        public ProcessorUsage(TimeSpan timeUsed, TimeSpan maxTimeAvailablePerProcessor, int processorCount)
        {
            TimeUsed = timeUsed;
            MaxTimeAvailablePerProcessor = maxTimeAvailablePerProcessor;
            ProcessorCount = processorCount;
        }

        public TimeSpan TimeUsed { get; }
        public TimeSpan MaxTimeAvailablePerProcessor { get; }
        public int ProcessorCount { get; }

        public double ProcessorFraction => TimeUsed.TotalMilliseconds / MaxTimeAvailablePerProcessor.TotalMilliseconds;
        public double ProcessorPercentage => ProcessorFraction * 100.0;
        public double TotalFraction => ProcessorFraction / ProcessorCount;
        public double TotalPercentage => TotalFraction * 100.0;
        public TimeSpan TotalTimeAvailable => TimeSpan.FromMilliseconds(ProcessorCount * TimeUsed.TotalMilliseconds);
    }
}
