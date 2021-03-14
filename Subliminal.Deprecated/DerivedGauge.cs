using System;

namespace Subliminal
{
    public sealed class DerivedGauge<TValue> : IGauge<TValue>
    {
        private readonly ILog<Measure<TValue>> _log;

        public DerivedGauge(ILog<Measure<TValue>> log)
        {
            _log = log;
        }

        public IDisposable Subscribe(ILogHandler<Measure<TValue>> logHandler)
        {
            return _log.Subscribe(logHandler);
        }
    }
}
