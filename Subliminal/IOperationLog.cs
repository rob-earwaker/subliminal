namespace Subliminal
{
    public interface IOperationLog
    {
        IEventLog<OperationStarted> OperationStarted { get; }
        IEventLog<OperationEnded> OperationEnded { get; }
        IEventLog<OperationCompleted> OperationCompleted { get; }
        IEventLog<OperationCanceled> OperationCanceled { get; }
    }
}
