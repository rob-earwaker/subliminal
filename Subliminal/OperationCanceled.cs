using System;

namespace Subliminal
{
    public class OperationCanceled<TContext>
    {
        internal OperationCanceled(Guid operationId, TContext context, TimeSpan duration)
        {
            OperationId = operationId;
            Context = context;
            Duration = duration;
        }

        public Guid OperationId { get; }
        public TContext Context { get; }
        public TimeSpan Duration { get; }

        internal OperationCanceled WithoutContext()
        {
            return new OperationCanceled(OperationId, Duration);
        }
    }

    public class OperationCanceled
    {
        internal OperationCanceled(Guid operationId, TimeSpan duration)
        {
            OperationId = operationId;
            Duration = duration;
        }

        public Guid OperationId { get; }
        public TimeSpan Duration { get; }
    }
}
