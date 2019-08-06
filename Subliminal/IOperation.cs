namespace Subliminal
{
    public interface IOperation<TContext>
    {
        IEventLog<StartedOperation<TContext>> Started { get; }
        IEventLog<EndedOperation<TContext>> Ended { get; }
        IEventLog<CompletedOperation<TContext>> Completed { get; }
        IEventLog<CanceledOperation<TContext>> Canceled { get; }
    }

    public interface IOperation
    {
        IEventLog<StartedOperation> Started { get; }
        IEventLog<EndedOperation> Ended { get; }
        IEventLog<CompletedOperation> Completed { get; }
        IEventLog<CanceledOperation> Canceled { get; }
    }
}
