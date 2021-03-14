namespace Subliminal
{
    public class Count : ILogEntry
    {
        public Count(double value, Context context, OperationSnapshot parentOperation)
        {
            Value = value;
            Context = context;
            ParentOperation = parentOperation;
        }

        public double Value { get; }
        public Context Context { get; }
        public OperationSnapshot ParentOperation { get; }
    }
}
