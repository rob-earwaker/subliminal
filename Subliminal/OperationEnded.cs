using System;

namespace Subliminal
{
    public class OperationEnded<TContext>
    {
        internal OperationEnded(Guid operationId, TContext context, TimeSpan duration, bool wasCanceled)
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

        internal OperationEnded WithoutContext()
        {
            return new OperationEnded(OperationId, Duration, WasCanceled);
        }
    }

    public class OperationEnded
    {
        internal OperationEnded(Guid operationId, TimeSpan duration, bool wasCanceled)
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
