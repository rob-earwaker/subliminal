using System;

namespace Subliminal
{
    public class OperationCanceled
    {
        public OperationCanceled(Guid operationId, TimeSpan duration)
        {
            OperationId = operationId;
            Duration = duration;
        }

        public Guid OperationId { get; }
        public TimeSpan Duration { get; }
    }
}
