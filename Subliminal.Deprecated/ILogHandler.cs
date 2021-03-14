using System;

namespace Subliminal
{
    public interface ILogHandler<in TLogEntry> where TLogEntry : ILogEntry
    {
        void Handle(TLogEntry logEntry);
    }
}
