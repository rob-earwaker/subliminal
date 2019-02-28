using System;

namespace Subliminal
{
    public class Process
    {
        public Process(TimeSpan totalProcessorTime, long workingSet, long privateMemorySize, long virtualMemorySize)
        {
            TotalProcessorTime = totalProcessorTime;
            WorkingSet = workingSet;
            PrivateMemorySize = privateMemorySize;
            VirtualMemorySize = virtualMemorySize;
        }

        public static Process FromCurrentProcess()
        {
            var process = System.Diagnostics.Process.GetCurrentProcess();

            return new Process(
                process.TotalProcessorTime,
                process.WorkingSet64,
                process.PrivateMemorySize64,
                process.VirtualMemorySize64);
        }

        public TimeSpan TotalProcessorTime { get; }
        public long WorkingSet { get; }
        public long PrivateMemorySize { get; }
        public long VirtualMemorySize { get; }
    }
}
