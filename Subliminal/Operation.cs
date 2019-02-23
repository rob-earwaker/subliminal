using Subliminal.Events;
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
            _started.OnNext(new OperationStarted(operationScope));
            return operationScope;
        }

        public IObservable<OperationStarted> Started => _started.AsObservable();
        public IObservable<OperationEnded> Ended => Started.SelectMany(started => started.Operation.Ended);
        public IObservable<OperationCompleted> Completed => Started.SelectMany(started => started.Operation.Completed);
        public IObservable<OperationCanceled> Canceled => Started.SelectMany(started => started.Operation.Canceled);
    }
}
