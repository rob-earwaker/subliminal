using System;

namespace Subliminal
{
    public class CompletedOperation<TContext>
    {
        public CompletedOperation(Guid operationId, TContext context, TimeSpan duration)
        {
            OperationId = operationId;
            Context = context;
            Duration = duration;
        }

        public Guid OperationId { get; }
        public TContext Context { get; }
        public TimeSpan Duration { get; }

        public CompletedOperation WithoutContext()
        {
            return new CompletedOperation(OperationId, Duration);
        }
    }

    public class CompletedOperation
    {
        public CompletedOperation(Guid operationId, TimeSpan duration)
        {
            OperationId = operationId;
            Duration = duration;
        }

        public Guid OperationId { get; }
        public TimeSpan Duration { get; }
    }
}
