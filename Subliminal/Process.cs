using System;

namespace Subliminal
{
    public class Process
    {
        public Process(
            TimeSpan totalProcessorTime, ByteCount workingSet,
            ByteCount privateMemorySize, ByteCount virtualMemorySize)
        {
            TotalProcessorTime = totalProcessorTime;
            WorkingSet = workingSet;
            PrivateMemorySize = privateMemorySize;
            VirtualMemorySize = virtualMemorySize;
        }

        public TimeSpan TotalProcessorTime { get; }
        public ByteCount WorkingSet { get; }
        public ByteCount PrivateMemorySize { get; }
        public ByteCount VirtualMemorySize { get; }
    }
}
