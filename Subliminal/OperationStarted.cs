using System.Reactive.Linq;

namespace Subliminal
{
    /// <summary>
    /// An event containing information about a started operation.
    /// </summary>
    public sealed class OperationStarted<TContext>
    {
        private readonly IEvent<TimerStopped> _timerStopped;

        internal OperationStarted(string operationId, TContext context, IEvent<TimerStopped> timerStopped)
        {
            OperationId = operationId;
            Context = context;
            _timerStopped = timerStopped;
        }

        /// <summary>
        /// An identifier for the operation.
        /// </summary>
        public string OperationId { get; }

        /// <summary>
        /// Context data associated with the operation.
        /// </summary>
        public TContext Context { get; }

        internal IEvent<OperationCompleted<TContext>> Completed
        {
            get
            {
                return _timerStopped
                    .Where(stopped => !stopped.WasCanceled)
                    .Select(stopped => new OperationCompleted<TContext>(OperationId, Context, stopped.Duration))
                    .AsEvent();
            }
        }

        internal IEvent<OperationCanceled<TContext>> Canceled
        {
            get
            {
                return _timerStopped
                    .Where(stopped => stopped.WasCanceled)
                    .Select(stopped => new OperationCanceled<TContext>(OperationId, Context))
                    .AsEvent();
            }
        }

        internal OperationStarted WithoutContext()
        {
            return new OperationStarted(
                OperationId,
                Completed.Select(completed => completed.WithoutContext()).AsEvent(),
                Canceled.Select(canceled => canceled.WithoutContext()).AsEvent());
        }
    }

    /// <summary>
    /// An event containing information about a started operation.
    /// </summary>
    public sealed class OperationStarted
    {
        internal OperationStarted(string operationId, IEvent<OperationCompleted> completed, IEvent<OperationCanceled> canceled)
        {
            OperationId = operationId;
            Completed = completed;
            Canceled = canceled;
        }

        /// <summary>
        /// An identifier for the operation.
        /// </summary>
        public string OperationId { get; }

        internal IEvent<OperationCompleted> Completed { get; }
        internal IEvent<OperationCanceled> Canceled { get; }
    }
}
