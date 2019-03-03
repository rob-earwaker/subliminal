using System;

namespace Subliminal
{
    public class OperationCompleted
    {
        public OperationCompleted(OperationId operationId, TimeSpan duration)
        {
            OperationId = operationId;
            Duration = duration;
        }

        public OperationId OperationId { get; }
        public TimeSpan Duration { get; }
    }
}
