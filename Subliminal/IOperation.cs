using System;

namespace Subliminal
{
    public interface IOperation
    {
        IEventLog<OperationStarted> Started { get; }
        IEventLog<OperationEnded> Ended { get; }
        IEventLog<OperationCompleted> Completed { get; }
        IEventLog<OperationCanceled> Canceled { get; }
        IMetric<TimeSpan> EndedDuration { get; }
        IMetric<TimeSpan> CompletedDuration { get; }
        IMetric<TimeSpan> CanceledDuration { get; }
    }
}
