using System;
using System.Diagnostics;

namespace Subliminal
{
    public class Execution : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly Event<OperationEnded> _ended;
        private bool _hasEnded;

        public Execution(Guid operationId)
        {
            OperationId = operationId;
            ExecutionId = Guid.NewGuid();

            _stopwatch = Stopwatch.StartNew();
            _ended = new Event<OperationEnded>();
            _hasEnded = false;

        }

        public Guid OperationId { get; }
        public Guid ExecutionId { get; }
        public IEvent<OperationEnded> Ended => _ended;

        public void Dispose()
        {
            Complete();
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
