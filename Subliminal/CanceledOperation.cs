using System;

namespace Subliminal
{
    public class CanceledOperation
    {
        public CanceledOperation(Guid operationId, TimeSpan duration)
        {
            OperationId = operationId;
            Duration = duration;
        }

        public Guid OperationId { get; }
        public TimeSpan Duration { get; }
    }
}
