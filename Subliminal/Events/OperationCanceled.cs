namespace Subliminal.Events
{
    public class OperationCanceled
    {
        public OperationCanceled(OperationScope operation)
        {
            Operation = operation;
        }

        public OperationScope Operation { get; }
    }
}
