using System;

namespace Subliminal
{
    public interface ILog<TEntry> : IObservable<TEntry>
    {
    }
}
