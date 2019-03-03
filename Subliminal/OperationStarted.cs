using System;

namespace Subliminal
{
    public class OperationStarted
    {
        public OperationStarted(Guid operationId, IEvent<OperationEnded> ended)
        {
            OperationId = operationId;
            Ended = ended;
        }

        public Guid OperationId { get; }
        public IEvent<OperationEnded> Ended { get; }
    }
}
