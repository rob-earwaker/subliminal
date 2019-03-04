using System;

namespace Subliminal
{
    public class OperationCompleted : ITiming
    {
        public OperationCompleted(Guid operationId, TimeSpan duration)
        {
            OperationId = operationId;
            Duration = duration;
        }

        public Guid OperationId { get; }
        public TimeSpan Duration { get; }
    }
}
