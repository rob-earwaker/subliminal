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
                return Ended.Activated
                    .Where(endedTrigger => !endedTrigger.Context.WasCanceled)
                    .Select(endedTrigger => new OperationCompleted(
                        endedTrigger.Context.OperationId, endedTrigger.Context.ExecutionId, endedTrigger.Context.Duration))
                    .AsTrigger();
            }
        }

        public ITrigger<OperationCanceled> Canceled
        {
            get
            {
                return Ended.Activated
                    .Where(endedTrigger => endedTrigger.Context.WasCanceled)
                    .Select(endedTrigger => new OperationCanceled(
                        endedTrigger.Context.OperationId, endedTrigger.Context.ExecutionId, endedTrigger.Context.Duration))
                    .AsTrigger();
            }
        }
    }
}
