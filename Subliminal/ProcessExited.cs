using System;

namespace Subliminal
{
    /// <summary>
    /// An event containing information about a process that has exited.
    /// </summary>
    public sealed class ProcessExited
    {
        /// <summary>
        /// Creates an event containing information about a process that has exited.
        /// </summary>
        public ProcessExited(int processId, DateTime exitTime, int exitCode)
        {
            ProcessId = processId;
            ExitTime = exitTime;
            ExitCode = exitCode;
        }

        /// <summary>
        /// The identifier for the process.
        /// </summary>
        public int ProcessId { get; }

        /// <summary>
        /// The time at which the process exited.
        /// </summary>
        public DateTime ExitTime { get; }

        /// <summary>
        /// The exit code returned by the process.
        /// </summary>
        public int ExitCode { get; }
    }
}