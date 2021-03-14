using System;

namespace Subliminal
{
    public sealed class Gauge<TValue> : IGauge<TValue>
    {
        private Log<Measure<TValue>> _log;

        public Gauge()
        {
            _log = new Log<Measure<TValue>>();
        }

        public void LogValue(TValue value)
        {
            _log.LogEntry(new Measure<TValue>(value, new Context(), Operation.SnapshotCurrentOperation()));
        }

        public IDisposable Subscribe(ILogHandler<Measure<TValue>> logHandler)
        {
            return _log.Subscribe(logHandler);
        }
    }
}
