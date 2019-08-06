using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Subliminal
{
    public class StartedOperation<TContext>
    {
        public StartedOperation(Guid operationId, TContext context, IEvent<EndedOperation<TContext>> ended)
        {
            OperationId = operationId;
            Context = context;
            Ended = ended;
        }

        public Guid OperationId { get; }
        public TContext Context { get; }
        public IEvent<EndedOperation<TContext>> Ended { get; }

        public IEvent<CompletedOperation<TContext>> Completed
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

        public IEvent<CanceledOperation<TContext>> Canceled
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

        public StartedOperation WithoutContext()
        {
            return new StartedOperation(
                OperationId,
                Ended.Select(operation => operation.WithoutContext()).AsEvent());
        }
    }

    public class StartedOperation
    {
        public StartedOperation(Guid operationId, IEvent<EndedOperation> ended)
        {
            OperationId = operationId;
            Ended = ended;
        }

        public Guid OperationId { get; }
        public IEvent<EndedOperation> Ended { get; }

        public IEvent<CompletedOperation> Completed
        {
            get
            {
                return Ended
                    .Where(operation => !operation.WasCanceled)
                    .Select(operation => new CompletedOperation(operation.OperationId, operation.Duration))
                    .AsEvent();
            }
        }

        public IEvent<CanceledOperation> Canceled
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
