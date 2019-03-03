using System;

namespace Subliminal
{
    public class Process
    {
        public Process(
            TimeSpan totalProcessorTime, SizeBytes workingSet,
            SizeBytes privateMemorySize, SizeBytes virtualMemorySize)
        {
            TotalProcessorTime = totalProcessorTime;
            WorkingSet = workingSet;
            PrivateMemorySize = privateMemorySize;
            VirtualMemorySize = virtualMemorySize;
        }

        public TimeSpan TotalProcessorTime { get; }
        public SizeBytes WorkingSet { get; }
        public SizeBytes PrivateMemorySize { get; }
        public SizeBytes VirtualMemorySize { get; }
    }
}
