using System;

namespace Subliminal
{
    public class EndedOperation<TContext>
    {
        internal EndedOperation(Guid operationId, TContext context, TimeSpan duration, bool wasCanceled)
        {
            OperationId = operationId;
            Context = context;
            Duration = duration;
            WasCanceled = wasCanceled;
        }

        public Guid OperationId { get; }
        public TContext Context { get; }
        public TimeSpan Duration { get; }
        public bool WasCanceled { get; }

        internal EndedOperation WithoutContext()
        {
            return new EndedOperation(OperationId, Duration, WasCanceled);
        }
    }

    public class EndedOperation
    {
        internal EndedOperation(Guid operationId, TimeSpan duration, bool wasCanceled)
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
