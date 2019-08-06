using System;

namespace Subliminal
{
    public class CanceledOperation<TContext>
    {
        public CanceledOperation(Guid operationId, TContext context, TimeSpan duration)
        {
            OperationId = operationId;
            Context = context;
            Duration = duration;
        }

        public Guid OperationId { get; }
        public TContext Context { get; }
        public TimeSpan Duration { get; }

        public CanceledOperation WithoutContext()
        {
            return new CanceledOperation(OperationId, Duration);
        }
    }

    public class CanceledOperation
    {
        public CanceledOperation(Guid operationId, TimeSpan duration)
        {
            OperationId = operationId;
            Duration = duration;
        }

        public Guid OperationId { get; }
        public TimeSpan Duration { get; }
    }
}
