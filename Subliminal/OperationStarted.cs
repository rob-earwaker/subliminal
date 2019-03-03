using System;

namespace Subliminal
{
    public class OperationStarted
    {
        public OperationStarted(OperationId operationId, IObservable<OperationEnded> ended)
        {
            OperationId = operationId;
            Ended = ended;
        }

        public OperationId OperationId { get; }
        public IObservable<OperationEnded> Ended { get; }
    }
}
