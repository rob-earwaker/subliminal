using System;

namespace Subliminal
{
    public interface IOperation
    {
        Guid OperationId { get; }
        IEventLog<OperationStarted> Started { get; }
        IEventLog<OperationEnded> Ended { get; }
        IEventLog<OperationCompleted> Completed { get; }
        IEventLog<OperationCanceled> Canceled { get; }
    }
}
