namespace Subliminal
{
    public class OperationCanceled<TContext>
    {
        internal OperationCanceled(string operationId, TContext context)
        {
            OperationId = operationId;
            Context = context;
        }

        public string OperationId { get; }
        public TContext Context { get; }

        internal OperationCanceled WithoutContext()
        {
            return new OperationCanceled(OperationId);
        }
    }

    public class OperationCanceled
    {
        internal OperationCanceled(string operationId)
        {
            OperationId = operationId;
        }

        public string OperationId { get; }
    }
}
