using System;

namespace Subliminal
{
    public class CompletedOperation
    {
        public CompletedOperation(Guid operationId, TimeSpan duration)
        {
            OperationId = operationId;
            Duration = duration;
        }

        public Guid OperationId { get; }
        public TimeSpan Duration { get; }
    }
}
