using System;
using System.Diagnostics;

namespace Subliminal
{
    public class Timer : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly Event<EndedOperation> _ended;
        private bool _hasEnded;

        internal Timer()
        {
            OperationId = Guid.NewGuid();

            _stopwatch = new Stopwatch();
            _ended = new Event<EndedOperation>();
            _hasEnded = false;
        }

        internal void Start()
        {
            _stopwatch.Start();
        }

        public Guid OperationId { get; }
        public IEvent<EndedOperation> Ended => _ended;

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
            _ended.Raise(new EndedOperation(OperationId, _stopwatch.Elapsed, wasCanceled));
            _hasEnded = true;
        }

        void IDisposable.Dispose()
        {
            Complete();
        }
    }
}
