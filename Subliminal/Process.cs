using System;

namespace Subliminal
{
    public class Process
    {
        public Process(
            int processId, TimeSpan totalProcessorTime, long workingSet,
            long privateMemorySize, long virtualMemorySize)
        {
            ProcessId = processId;
            TotalProcessorTime = totalProcessorTime;
            WorkingSet = workingSet;
            PrivateMemorySize = privateMemorySize;
            VirtualMemorySize = virtualMemorySize;
        }

        public int ProcessId { get; }
        public TimeSpan TotalProcessorTime { get; }
        public long WorkingSet { get; }
        public long PrivateMemorySize { get; }
        public long VirtualMemorySize { get; }
    }
}
