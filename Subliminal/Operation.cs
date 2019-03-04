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

        public ITimingEventLog<OperationEnded> Ended
        {
            get
            {
                return Started
                    .SelectMany(operationStarted => operationStarted.Ended)
                    .AsTimingEventLog();
            }
        }

        public ITimingEventLog<OperationCompleted> Completed
        {
            get
            {
                return Ended
                    .Where(operationEnded => !operationEnded.WasCanceled)
                    .Select(operationEnded => new OperationCompleted(operationEnded.OperationId, operationEnded.Duration))
                    .AsTimingEventLog();
            }
        }

        public ITimingEventLog<OperationCanceled> Canceled
        {
            get
            {
                return Ended
                    .Where(operationEnded => operationEnded.WasCanceled)
                    .Select(operationEnded => new OperationCanceled(operationEnded.OperationId, operationEnded.Duration))
                    .AsTimingEventLog();
            }
        }
    }
}
