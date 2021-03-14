namespace Subliminal
{
    public class Measure<TValue> : ILogEntry
    {
        public Measure(TValue value, Context context, OperationSnapshot parentOperation)
        {
            Value = value;
            Context = context;
            ParentOperation = parentOperation;
        }

        public TValue Value { get; }
        public Context Context { get; }
        public OperationSnapshot ParentOperation { get; }
    }
}
