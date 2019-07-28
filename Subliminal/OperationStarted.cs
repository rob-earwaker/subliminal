using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class OperationStarted
    {
        public OperationStarted(Guid operationId, Guid executionId, ITrigger<OperationEnded> ended)
        {
            OperationId = operationId;
            ExecutionId = executionId;
            Ended = ended;
        }

        public Guid OperationId { get; }
        public Guid ExecutionId { get; }
        public ITrigger<OperationEnded> Ended { get; }

        public ITrigger<OperationCompleted> Completed
        {
            get
            {
                return Ended
                    .Where(operationEnded => !operationEnded.WasCanceled)
                    .Select(operationEnded => new OperationCompleted(
                        operationEnded.OperationId, operationEnded.ExecutionId, operationEnded.Duration))
                    .AsEvent();
            }
        }

        public ITrigger<OperationCanceled> Canceled
        {
            get
            {
                return Ended
                    .Where(operationEnded => operationEnded.WasCanceled)
                    .Select(operationEnded => new OperationCanceled(
                        operationEnded.OperationId, operationEnded.ExecutionId, operationEnded.Duration))
                    .AsEvent();
            }
        }
    }
}
