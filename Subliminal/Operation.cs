using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class Operation : IOperation
    {
        private readonly EventLog<OperationStarted> _started;

        public Operation()
        {
            _started = new EventLog<OperationStarted>();
        }

        public RunningOperation StartNew()
        {
            var runningOperation = new RunningOperation();
            _started.Log(new OperationStarted(runningOperation.OperationId, runningOperation.Ended));
            return runningOperation;
        }

        public IEventLog<OperationStarted> Started => _started;

        public IEventLog<OperationEnded> Ended
        {
            get
            {
                return Started
                    .SelectMany(operationStarted => operationStarted.Ended)
                    .AsEventLog();
            }
        }

        public IEventLog<OperationCompleted> Completed
        {
            get
            {
                return Ended
                    .Where(operationEnded => !operationEnded.WasCanceled)
                    .Select(operationEnded => new OperationCompleted(operationEnded.OperationId, operationEnded.Duration))
                    .AsEventLog();
            }
        }

        public IEventLog<OperationCanceled> Canceled
        {
            get
            {
                return Ended
                    .Where(operationEnded => operationEnded.WasCanceled)
                    .Select(operationEnded => new OperationCanceled(operationEnded.OperationId, operationEnded.Duration))
                    .AsEventLog();
            }
        }

        public IMetric<TimeSpan> EndedDuration
        {
            get
            {
                return Ended
                    .Select(operationEnded => operationEnded.Duration)
                    .AsMetric();
            }
        }

        public IMetric<TimeSpan> CompletedDuration
        {
            get
            {
                return Completed
                    .Select(operationCompleted => operationCompleted.Duration)
                    .AsMetric();
            }
        }

        public IMetric<TimeSpan> CanceledDuration
        {
            get
            {
                return Canceled
                    .Select(operationCanceled => operationCanceled.Duration)
                    .AsMetric();
            }
        }
    }
}
