using System;
using System.Diagnostics;

namespace Subliminal
{
    public class OperationScope : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly Event<OperationEnded> _ended;
        private bool _hasStarted;
        private bool _hasEnded;

        public OperationScope()
        {
            OperationId = OperationId.New();

            _stopwatch = new Stopwatch();
            _ended = new Event<OperationEnded>();
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

        public IEvent<OperationEnded> Ended => _ended;

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

            _ended.LogAndClose(new OperationEnded(OperationId, _stopwatch.Elapsed, wasCanceled));

            _hasEnded = true;
        }
    }
}
