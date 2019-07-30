using System;

namespace Subliminal
{
    public class ExitedProcess
    {
        public ExitedProcess(int processId, DateTime exitTime, int exitCode)
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