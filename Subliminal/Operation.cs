using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class Operation : IDisposable
    {
        private readonly Subject<OperationScope> _started;

        public Operation()
        {
            _started = new Subject<OperationScope>();
        }

        public IObservable<OperationScope> Started => _started.AsObservable();

        public IObservable<OperationScope> Completed
        {
            get { return Started.SelectMany(operationScope => operationScope.Completed.Select(_ => operationScope)); }
        }

        public IObservable<OperationScope> Canceled
        {
            get { return Started.SelectMany(operationScope => operationScope.Canceled.Select(_ => operationScope)); }
        }

        public OperationScope StartNew()
        {
            var operationScope = OperationScope.StartNew();
            _started.OnNext(operationScope);
            return operationScope;
        }

        public void Dispose()
        {
            _started?.Dispose();
        }
    }
}
