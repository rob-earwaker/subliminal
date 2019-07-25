using System;

namespace Subliminal
{
    public class OperationCanceled
    {
        public OperationCanceled(Guid operationId, Guid executionId, TimeSpan duration)
        {
            OperationId = operationId;
            ExecutionId = executionId;
            Duration = duration;
        }

        public Guid OperationId { get; }
        public Guid ExecutionId { get; }
        public TimeSpan Duration { get; }
    }
}
