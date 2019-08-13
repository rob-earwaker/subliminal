using System;

namespace Subliminal
{
    public class CompletedOperation<TContext>
    {
        internal CompletedOperation(Guid operationId, TContext context, TimeSpan duration)
        {
            OperationId = operationId;
            Context = context;
            Duration = duration;
        }

        public Guid OperationId { get; }
        public TContext Context { get; }
        public TimeSpan Duration { get; }

        internal CompletedOperation WithoutContext()
        {
            return new CompletedOperation(OperationId, Duration);
        }
    }

    public class CompletedOperation
    {
        internal CompletedOperation(Guid operationId, TimeSpan duration)
        {
            OperationId = operationId;
            Duration = duration;
        }

        public Guid OperationId { get; }
        public TimeSpan Duration { get; }
    }
}
