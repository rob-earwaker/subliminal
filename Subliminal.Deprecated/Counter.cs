using System;

namespace Subliminal
{
    public sealed class Counter : ICounter
    {
        private readonly Log<Count> _log;

        public Counter()
        {
            _log = new Log<Count>();
        }

        public void IncrementBy(double value)
        {
            _log.LogEntry(new Count(value, new Context(), Operation.SnapshotCurrentOperation()));
        }

        public void Increment()
        {
            IncrementBy(1);
        }

        public IDisposable Subscribe(ILogHandler<Count> logHandler)
        {
            return _log.Subscribe(logHandler);
        }
    }
}
