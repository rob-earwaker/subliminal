using System;

namespace Subliminal
{
    public class Process
    {
        public Process(
            int processId, TimeSpan totalProcessorTime, ByteCount workingSet,
            ByteCount privateMemorySize, ByteCount virtualMemorySize)
        {
            ProcessId = processId;
            TotalProcessorTime = totalProcessorTime;
            WorkingSet = workingSet;
            PrivateMemorySize = privateMemorySize;
            VirtualMemorySize = virtualMemorySize;
        }

        public int ProcessId { get; }
        public TimeSpan TotalProcessorTime { get; }
        public ByteCount WorkingSet { get; }
        public ByteCount PrivateMemorySize { get; }
        public ByteCount VirtualMemorySize { get; }
    }
}
