using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class OperationScope : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly Subject<OperationEnded> _ended;
        private bool _hasStarted;
        private bool _hasEnded;

        public OperationScope()
        {
            OperationId = OperationId.New();

            _stopwatch = new Stopwatch();
            _ended = new Subject<OperationEnded>();
            _hasStarted = false;
            _hasEnded = false;
        }

        public OperationId OperationId { get; }

        public static OperationScope StartNew()
        {
            var operationTimer = new OperationScope();
            operationTimer.Start();
            return operationTimer;
        }

        public IObservable<OperationEnded> Ended => _ended.AsObservable();

        public void Start()
        {
            if (_hasStarted)
                return;

            _stopwatch.Start();

            _hasStarted = true;
        }

        public void End()
        {
            End(wasCanceled: false);
        }

        public void Cancel()
        {
            End(wasCanceled: true);
        }

        public void Dispose()
        {
            End();
        }

        private void End(bool wasCanceled)
        {
            if (!_hasStarted || _hasEnded)
                return;

            _stopwatch.Stop();

            _ended.OnNext(new OperationEnded(OperationId, _stopwatch.Elapsed, wasCanceled));
            _ended.OnCompleted();

            _hasEnded = true;
        }
    }
}
