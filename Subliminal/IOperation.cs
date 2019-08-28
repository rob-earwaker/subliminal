namespace Subliminal
{
    /// <summary>
    /// A group of metrics relating to execution of an operation.
    /// </summary>
    public interface IOperation<TContext>
    {
        /// <summary>
        /// An event log that emits an event every time a new operation timer is started.
        /// </summary>
        IEventLog<OperationStarted<TContext>> Started { get; }

        /// <summary>
        /// An event log that emits an event every time an operation timer is completed.
        /// </summary>
        IEventLog<OperationCompleted<TContext>> Completed { get; }

        /// <summary>
        /// An event log that emits an event every time an operation timer is canceled.
        /// </summary>
        IEventLog<OperationCanceled<TContext>> Canceled { get; }
    }

    /// <summary>
    /// A group of metrics relating to execution of an operation.
    /// </summary>
    public interface IOperation
    {
        /// <summary>
        /// An event log that emits an event every time a new operation timer is started.
        /// </summary>
        IEventLog<OperationStarted> Started { get; }

        /// <summary>
        /// An event log that emits an event every time an operation timer is completed.
        /// </summary>
        IEventLog<OperationCompleted> Completed { get; }

        /// <summary>
        /// An event log that emits an event every time an operation timer is canceled.
        /// </summary>
        IEventLog<OperationCanceled> Canceled { get; }
    }
}
