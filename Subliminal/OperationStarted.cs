using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class OperationStarted<TContext>
    {
        internal OperationStarted(Guid operationId, TContext context, IEvent<OperationEnded<TContext>> ended)
        {
            OperationId = operationId;
            Context = context;
            Ended = ended;
        }

        public Guid OperationId { get; }
        public TContext Context { get; }

        internal IEvent<OperationEnded<TContext>> Ended { get; }

        internal IEvent<OperationCompleted<TContext>> Completed
        {
            get
            {
                return Ended
                    .Where(operation => !operation.WasCanceled)
                    .Select(operation => new OperationCompleted<TContext>(
                        operation.OperationId, operation.Context, operation.Duration))
                    .AsEvent();
            }
        }

        internal IEvent<OperationCanceled<TContext>> Canceled
        {
            get
            {
                return Ended
                    .Where(operation => operation.WasCanceled)
                    .Select(operation => new OperationCanceled<TContext>(
                        operation.OperationId, operation.Context, operation.Duration))
                    .AsEvent();
            }
        }

        internal OperationStarted WithoutContext()
        {
            return new OperationStarted(
                OperationId,
                Ended.Select(operation => operation.WithoutContext()).AsEvent());
        }
    }

    public class OperationStarted
    {
        internal OperationStarted(Guid operationId, IEvent<OperationEnded> ended)
        {
            OperationId = operationId;
            Ended = ended;
        }

        public Guid OperationId { get; }

        internal IEvent<OperationEnded> Ended { get; }

        internal IEvent<OperationCompleted> Completed
        {
            get
            {
                return Ended
                    .Where(operation => !operation.WasCanceled)
                    .Select(operation => new OperationCompleted(operation.OperationId, operation.Duration))
                    .AsEvent();
            }
        }

        internal IEvent<OperationCanceled> Canceled
        {
            get
            {
                return Ended
                    .Where(operation => operation.WasCanceled)
                    .Select(operation => new OperationCanceled(operation.OperationId, operation.Duration))
                    .AsEvent();
            }
        }
    }
}
