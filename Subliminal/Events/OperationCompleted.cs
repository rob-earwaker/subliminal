namespace Subliminal.Events
{
    public class OperationCompleted
    {
        public OperationCompleted(OperationScope operation)
        {
            Operation = operation;
        }

        public OperationScope Operation { get; }
    }
}
