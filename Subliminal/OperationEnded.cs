using System;

namespace Subliminal
{
    public class OperationEnded
    {
        public OperationEnded(OperationId operationId, TimeSpan duration, bool wasCanceled)
        {
            OperationId = operationId;
            Duration = duration;
            WasCanceled = wasCanceled;
        }

        public OperationId OperationId { get; }
        public TimeSpan Duration { get; }
        public bool WasCanceled { get; }
    }
}
