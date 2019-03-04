using System;

namespace Subliminal
{
    public class OperationStarted
    {
        public OperationStarted(Guid operationId, ITimingEvent<OperationEnded> ended)
        {
            OperationId = operationId;
            Ended = ended;
        }

        public Guid OperationId { get; }
        public ITimingEvent<OperationEnded> Ended { get; }
    }
}
