namespace Subliminal.Events
{
    public class OperationEnded
    {
        public OperationEnded(OperationScope operation)
        {
            Operation = operation;
        }

        public OperationScope Operation { get; }
    }
}
