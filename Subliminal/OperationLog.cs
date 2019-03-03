using System.Reactive.Linq;

namespace Subliminal
{
    public class OperationLog : IOperationLog
    {
        private readonly EventLog<OperationStarted> _operationStarted;

        public OperationLog()
        {
            _operationStarted = new EventLog<OperationStarted>();
        }

        public Operation StartNew()
        {
            var operationScope = Operation.StartNew();
            _operationStarted.Log(new OperationStarted(operationScope.OperationId, operationScope.EndedEvent));
            return operationScope;
        }

        public IEventLog<OperationStarted> OperationStarted => _operationStarted;

        public IEventLog<OperationEnded> OperationEnded
        {
            get
            {
                return OperationStarted
                    .SelectMany(operationStarted => operationStarted.EndedEvent)
                    .AsEventLog();
            }
        }

        public IEventLog<OperationCompleted> OperationCompleted
        {
            get
            {
                return OperationEnded
                    .Where(operationEnded => !operationEnded.WasCanceled)
                    .Select(operationEnded => new OperationCompleted(operationEnded.OperationId, operationEnded.Duration))
                    .AsEventLog();
            }
        }

        public IEventLog<OperationCanceled> OperationCanceled
        {
            get
            {
                return OperationEnded
                    .Where(operationEnded => operationEnded.WasCanceled)
                    .Select(operationEnded => new OperationCanceled(operationEnded.OperationId, operationEnded.Duration))
                    .AsEventLog();
            }
        }
    }
}
