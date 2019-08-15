namespace Subliminal
{
    public interface IOperation<TContext>
    {
        IEventLog<OperationStarted<TContext>> Started { get; }
        IEventLog<OperationCompleted<TContext>> Completed { get; }
        IEventLog<OperationCanceled<TContext>> Canceled { get; }
    }

    public interface IOperation
    {
        IEventLog<OperationStarted> Started { get; }
        IEventLog<OperationCompleted> Completed { get; }
        IEventLog<OperationCanceled> Canceled { get; }
    }
}
