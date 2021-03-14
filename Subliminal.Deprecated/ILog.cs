using System;

namespace Subliminal
{
    public interface ILog<out TLogEntry> where TLogEntry : ILogEntry
    {
        IDisposable Subscribe(ILogHandler<TLogEntry> logHandler);
    }
}
