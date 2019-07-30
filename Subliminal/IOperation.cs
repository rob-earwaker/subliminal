namespace Subliminal
{
    public interface IOperation
    {
        IEventLog<StartedOperation> Started { get; }
        IEventLog<EndedOperation> Ended { get; }
        IEventLog<CompletedOperation> Completed { get; }
        IEventLog<CanceledOperation> Canceled { get; }
    }
}
