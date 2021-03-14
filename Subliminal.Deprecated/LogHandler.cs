using System;

namespace Subliminal
{
    public class LogHandler<TLogEntry> : ILogHandler<TLogEntry> where TLogEntry : ILogEntry
    {
        private readonly Action<TLogEntry> _handle;

        public LogHandler(Action<TLogEntry> handle)
        {
            _handle = handle;
        }

        public void Handle(TLogEntry logEntry)
        {
            _handle(logEntry);
        }
    }
}
