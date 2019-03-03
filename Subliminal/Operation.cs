using System.Reactive.Linq;

namespace Subliminal
{
    public class Operation
    {
        private readonly EventLog<OperationStarted> _started;

        public Operation()
        {
            _started = new EventLog<OperationStarted>();
        }

        public OperationScope StartNew()
        {
            var operationScope = OperationScope.StartNew();
            _started.Log(new OperationStarted(operationScope.OperationId, operationScope.Ended));
            return operationScope;
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
    }
}
