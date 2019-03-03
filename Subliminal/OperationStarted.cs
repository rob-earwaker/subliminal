namespace Subliminal
{
    public class OperationStarted
    {
        public OperationStarted(OperationId operationId, IEvent<OperationEnded> endedEvent)
        {
            OperationId = operationId;
            EndedEvent = endedEvent;
        }

        public OperationId OperationId { get; }
        public IEvent<OperationEnded> EndedEvent { get; }
    }
}
