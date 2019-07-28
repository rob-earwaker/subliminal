using System;
using System.Diagnostics;

namespace Subliminal
{
    public class ExecutionTimer : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly Trigger<OperationEnded> _ended;
        private bool _hasEnded;

        public ExecutionTimer(Guid operationId)
        {
            OperationId = operationId;
            ExecutionId = Guid.NewGuid();

            _stopwatch = Stopwatch.StartNew();
            _ended = new Trigger<OperationEnded>();
            _hasEnded = false;
        }

        public Guid OperationId { get; }
        public Guid ExecutionId { get; }
        public ITrigger<OperationEnded> Ended => _ended;

        void IDisposable.Dispose()
        {
            Complete();
        }

        public void Pause()
        {
            _stopwatch.Stop();
        }

        public void Resume()
        {
            _stopwatch.Start();
        }

        public void Complete()
        {
            End(wasCanceled: false);
        }

        public void Cancel()
        {
            End(wasCanceled: true);
        }

        private void End(bool wasCanceled)
        {
            if (_hasEnded)
                return;

            _stopwatch.Stop();
            _ended.Raise(new OperationEnded(OperationId, ExecutionId, _stopwatch.Elapsed, wasCanceled));
            _hasEnded = true;
        }
    }
}
