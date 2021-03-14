using System;

namespace Subliminal
{
    /// <summary>
    /// A metric containing information about a running process.
    /// </summary>
    public sealed class Process
    {
        /// <summary>
        /// Creates a metric containing information about a running process.
        /// </summary>
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

        /// <summary>
        /// The identifier for the process.
        /// </summary>
        public int ProcessId { get; }

        /// <summary>
        /// The total processor time used by the process.
        /// </summary>
        public TimeSpan TotalProcessorTime { get; }

        /// <summary>
        /// The current working set usage of the process in bytes.
        /// </summary>
        public long WorkingSet { get; }

        /// <summary>
        /// The current private memory size of the process in bytes.
        /// </summary>
        public long PrivateMemorySize { get; }

        /// <summary>
        /// The current virtual memory size of the process in bytes.
        /// </summary>
        public long VirtualMemorySize { get; }
    }
}
