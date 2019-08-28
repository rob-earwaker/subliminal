using System;

namespace Subliminal
{
    /// <summary>
    /// An event containing information about a completed operation.
    /// </summary>
    public sealed class OperationCompleted<TContext>
    {
        internal OperationCompleted(string operationId, TContext context, TimeSpan duration)
        {
            OperationId = operationId;
            Context = context;
            Duration = duration;
        }

        /// <summary>
        /// An identifier for the operation.
        /// </summary>
        public string OperationId { get; }

        /// <summary>
        /// Context data associated with the operation.
        /// </summary>
        public TContext Context { get; }

        /// <summary>
        /// The time taken to complete the operation.
        /// </summary>
        public TimeSpan Duration { get; }

        internal OperationCompleted WithoutContext()
        {
            return new OperationCompleted(OperationId, Duration);
        }
    }

    /// <summary>
    /// An event containing information about a completed operation.
    /// </summary>
    public sealed class OperationCompleted
    {
        internal OperationCompleted(string operationId, TimeSpan duration)
        {
            OperationId = operationId;
            Duration = duration;
        }

        /// <summary>
        /// An identifier for the operation.
        /// </summary>
        public string OperationId { get; }

        /// <summary>
        /// The time taken to complete the operation.
        /// </summary>
        public TimeSpan Duration { get; }
    }
}
