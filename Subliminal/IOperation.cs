namespace Subliminal
{
    public interface IOperation
    {
        IEventLog<OperationStarted> Started { get; }
        ITimingEventLog<OperationEnded> Ended { get; }
        ITimingEventLog<OperationCompleted> Completed { get; }
        ITimingEventLog<OperationCanceled> Canceled { get; }
    }
}
