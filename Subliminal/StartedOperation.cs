using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class StartedOperation<TContext>
    {
        internal StartedOperation(Guid operationId, TContext context, IEvent<EndedOperation<TContext>> ended)
        {
            OperationId = operationId;
            Context = context;
            Ended = ended;
        }

        public Guid OperationId { get; }
        public TContext Context { get; }

        internal IEvent<EndedOperation<TContext>> Ended { get; }

        internal IEvent<CompletedOperation<TContext>> Completed
        {
            get
            {
                return Ended
                    .Where(operation => !operation.WasCanceled)
                    .Select(operation => new CompletedOperation<TContext>(
                        operation.OperationId, operation.Context, operation.Duration))
                    .AsEvent();
            }
        }

        internal IEvent<CanceledOperation<TContext>> Canceled
        {
            get
            {
                return Ended
                    .Where(operation => operation.WasCanceled)
                    .Select(operation => new CanceledOperation<TContext>(
                        operation.OperationId, operation.Context, operation.Duration))
                    .AsEvent();
            }
        }

        internal StartedOperation WithoutContext()
        {
            return new StartedOperation(
                OperationId,
                Ended.Select(operation => operation.WithoutContext()).AsEvent());
        }
    }

    public class StartedOperation
    {
        internal StartedOperation(Guid operationId, IEvent<EndedOperation> ended)
        {
            OperationId = operationId;
            Ended = ended;
        }

        public Guid OperationId { get; }

        internal IEvent<EndedOperation> Ended { get; }

        internal IEvent<CompletedOperation> Completed
        {
            get
            {
                return Ended
                    .Where(operation => !operation.WasCanceled)
                    .Select(operation => new CompletedOperation(operation.OperationId, operation.Duration))
                    .AsEvent();
            }
        }

        internal IEvent<CanceledOperation> Canceled
        {
            get
            {
                return Ended
                    .Where(operation => operation.WasCanceled)
                    .Select(operation => new CanceledOperation(operation.OperationId, operation.Duration))
                    .AsEvent();
            }
        }
    }
}
