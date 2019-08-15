using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class OperationStarted<TContext>
    {
        private readonly IEvent<TimerStopped> _timerStopped;

        internal OperationStarted(Guid operationId, TContext context, IEvent<TimerStopped> timerStopped)
        {
            OperationId = operationId;
            Context = context;
            _timerStopped = timerStopped;
        }

        public Guid OperationId { get; }
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
                    .Select(stopped => new OperationCanceled<TContext>(OperationId, Context, stopped.Duration))
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

    public class OperationStarted
    {
        internal OperationStarted(Guid operationId, IEvent<OperationCompleted> completed, IEvent<OperationCanceled> canceled)
        {
            OperationId = operationId;
            Completed = completed;
            Canceled = canceled;
        }

        public Guid OperationId { get; }
        internal IEvent<OperationCompleted> Completed { get; }
        internal IEvent<OperationCanceled> Canceled { get; }
    }
}
