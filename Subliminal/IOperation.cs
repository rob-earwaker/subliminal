namespace Subliminal
{
    public interface IOperation
    {
        IEventLog<OperationStarted> Started { get; }
        IEventLog<OperationEnded> Ended { get; }
        IEventLog<OperationCompleted> Completed { get; }
        IEventLog<OperationCanceled> Canceled { get; }
        ITimer EndedTimer { get; }
        ITimer CompletedTimer { get; }
        ITimer CanceledTimer { get; }
    }
}
