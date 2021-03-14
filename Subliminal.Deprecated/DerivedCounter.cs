using System;

namespace Subliminal
{
    public sealed class DerivedCounter : ICounter
    {
        private readonly ILog<Count> _log;

        public DerivedCounter(ILog<Count> log)
        {
            _log = log;
        }

        public IDisposable Subscribe(ILogHandler<Count> logHandler)
        {
            return _log.Subscribe(logHandler);
        }
    }
}
