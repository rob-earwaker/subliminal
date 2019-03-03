using System.Reactive.Linq;

namespace Subliminal
{
    public class Operation
    {
        private readonly EventLog<OperationStarted> _startedEventLog;

        public Operation()
        {
            _startedEventLog = new EventLog<OperationStarted>();
        }

        public OperationScope StartNew()
        {
            var operationScope = OperationScope.StartNew();
            _startedEventLog.Log(new OperationStarted(operationScope.OperationId, operationScope.EndedEvent));
            return operationScope;
        }

        public IEventLog<OperationStarted> StartedEventLog => _startedEventLog;

        public IEventLog<OperationEnded> EndedEventLog
        {
            get
            {
                return StartedEventLog
                    .SelectMany(operationStarted => operationStarted.EndedEvent)
                    .AsEventLog();
            }
        }

        public IEventLog<OperationCompleted> CompletedEventLog
        {
            get
            {
                return EndedEventLog
                    .Where(operationEnded => !operationEnded.WasCanceled)
                    .Select(operationEnded => new OperationCompleted(operationEnded.OperationId, operationEnded.Duration))
                    .AsEventLog();
            }
        }

        public IEventLog<OperationCanceled> CanceledEventLog
        {
            get
            {
                return EndedEventLog
                    .Where(operationEnded => operationEnded.WasCanceled)
                    .Select(operationEnded => new OperationCanceled(operationEnded.OperationId, operationEnded.Duration))
                    .AsEventLog();
            }
        }
    }
}
