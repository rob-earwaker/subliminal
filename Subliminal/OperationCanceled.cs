using System;

namespace Subliminal
{
    public class OperationCanceled
    {
        public OperationCanceled(OperationId operationId, TimeSpan duration)
        {
            OperationId = operationId;
            Duration = duration;
        }

        public OperationId OperationId { get; }
        public TimeSpan Duration { get; }
    }
}
