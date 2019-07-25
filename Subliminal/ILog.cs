using System;

namespace Subliminal
{
    public interface ILog<TEntry>
    {
        Guid LogId { get; }
        IObservable<TEntry> Entries { get; }
    }
}
