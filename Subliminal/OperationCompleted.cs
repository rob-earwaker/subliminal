using System;

namespace Subliminal
{
    public class OperationCompleted<TContext>
    {
        internal OperationCompleted(Guid operationId, TContext context, TimeSpan duration)
        {
            OperationId = operationId;
            Context = context;
            Duration = duration;
        }

        public Guid OperationId { get; }
        public TContext Context { get; }
        public TimeSpan Duration { get; }

        internal OperationCompleted WithoutContext()
        {
            return new OperationCompleted(OperationId, Duration);
        }
    }

    public class OperationCompleted
    {
        internal OperationCompleted(Guid operationId, TimeSpan duration)
        {
            OperationId = operationId;
            Duration = duration;
        }

        public Guid OperationId { get; }
        public TimeSpan Duration { get; }
    }
}
