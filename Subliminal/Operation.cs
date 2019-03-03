using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class Operation
    {
        private readonly Subject<OperationStarted> _started;

        public Operation()
        {
            _started = new Subject<OperationStarted>();
        }

        public OperationScope StartNew()
        {
            var operationScope = OperationScope.StartNew();
            _started.OnNext(new OperationStarted(operationScope.OperationId, operationScope.Ended));
            return operationScope;
        }

        public IObservable<OperationStarted> Started => _started.AsObservable();

        public IObservable<OperationEnded> Ended => Started.SelectMany(operationStarted => operationStarted.Ended);

        public IObservable<OperationCompleted> Completed
        {
            get
            {
                return Ended
                    .Where(operationEnded => !operationEnded.WasCanceled)
                    .Select(operationEnded => new OperationCompleted(operationEnded.OperationId, operationEnded.Duration));
            }
        }

        public IObservable<OperationCanceled> Canceled
        {
            get
            {
                return Ended
                    .Where(operationEnded => operationEnded.WasCanceled)
                    .Select(operationEnded => new OperationCanceled(operationEnded.OperationId, operationEnded.Duration));
            }
        }
    }
}
