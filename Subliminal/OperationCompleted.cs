using System;

namespace Subliminal
{
    public class OperationCompleted<TContext>
    {
        internal OperationCompleted(string operationId, TContext context, TimeSpan duration)
        {
            OperationId = operationId;
            Context = context;
            Duration = duration;
        }

        public string OperationId { get; }
        public TContext Context { get; }
        public TimeSpan Duration { get; }

        internal OperationCompleted WithoutContext()
        {
            return new OperationCompleted(OperationId, Duration);
        }
    }

    public class OperationCompleted
    {
        internal OperationCompleted(string operationId, TimeSpan duration)
        {
            OperationId = operationId;
            Duration = duration;
        }

        public string OperationId { get; }
        public TimeSpan Duration { get; }
    }
}
