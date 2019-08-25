using System;

namespace Subliminal
{
    public sealed class ProcessExited
    {
        public ProcessExited(int processId, DateTime exitTime, int exitCode)
        {
            ProcessId = processId;
            ExitTime = exitTime;
            ExitCode = exitCode;
        }

        public int ProcessId { get; }
        public DateTime ExitTime { get; }
        public int ExitCode { get; }
    }
}