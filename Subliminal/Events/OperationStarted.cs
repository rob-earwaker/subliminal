namespace Subliminal.Events
{
    public class OperationStarted
    {
        public OperationStarted(OperationScope operation)
        {
            Operation = operation;
        }

        public OperationScope Operation { get; }
    }
}
