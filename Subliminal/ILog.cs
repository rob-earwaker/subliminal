using System;

namespace Subliminal
{
    public interface ILog<TValue>
    {
        Guid LogId { get; }
        IObservable<LogEntry<TValue>> EntryLogged { get; }
    }
}
