namespace Subliminal
{
    /// <summary>
    /// An event containing information about a canceled operation.
    /// </summary>
    public sealed class OperationCanceled<TContext>
    {
        internal OperationCanceled(string operationId, TContext context)
        {
            OperationId = operationId;
            Context = context;
        }

        /// <summary>
        /// An identifier for the operation.
        /// </summary>
        public string OperationId { get; }

        /// <summary>
        /// Context data associated with the operation.
        /// </summary>
        public TContext Context { get; }

        internal OperationCanceled WithoutContext()
        {
            return new OperationCanceled(OperationId);
        }
    }

    /// <summary>
    /// An event containing information about a canceled operation.
    /// </summary>
    public sealed class OperationCanceled
    {
        internal OperationCanceled(string operationId)
        {
            OperationId = operationId;
        }

        /// <summary>
        /// An identifier for the operation.
        /// </summary>
        public string OperationId { get; }
    }
}
