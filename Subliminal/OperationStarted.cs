﻿using System;
using System.Reactive.Linq;

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

        public IEvent<OperationCompleted> Completed
        {
            get
            {
                return Ended
                    .Where(operationEnded => !operationEnded.WasCanceled)
                    .Select(operationEnded => new OperationCompleted(operationEnded.OperationId, operationEnded.Duration))
                    .AsEvent();
            }
        }

        public IEvent<OperationCanceled> Canceled
        {
            get
            {
                return Ended
                    .Where(operationEnded => operationEnded.WasCanceled)
                    .Select(operationEnded => new OperationCanceled(operationEnded.OperationId, operationEnded.Duration))
                    .AsEvent();
            }
        }
    }
}
