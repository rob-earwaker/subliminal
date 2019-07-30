using System;

namespace Subliminal
{
    public class EndedOperation
    {
        public EndedOperation(Guid operationId, TimeSpan duration, bool wasCanceled)
        {
            OperationId = operationId;
            Duration = duration;
            WasCanceled = wasCanceled;
        }

        public Guid OperationId { get; }
        public TimeSpan Duration { get; }
        public bool WasCanceled { get; }
    }
}
