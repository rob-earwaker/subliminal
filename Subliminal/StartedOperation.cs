using System;
using System.Reactive.Linq;

namespace Subliminal
{
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
