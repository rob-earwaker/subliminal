using System;

namespace Subliminal
{
    public class OperationEnded : ITiming
    {
        public OperationEnded(Guid operationId, TimeSpan duration, bool wasCanceled)
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
