using System;

namespace Subliminal
{
    public class OperationEnded
    {
        public OperationEnded(Guid operationId, Guid executionId, TimeSpan duration, bool wasCanceled)
        {
            OperationId = operationId;
            ExecutionId = executionId;
            Duration = duration;
            WasCanceled = wasCanceled;
        }

        public Guid OperationId { get; }
        public Guid ExecutionId { get; }
        public TimeSpan Duration { get; }
        public bool WasCanceled { get; }
    }
}
