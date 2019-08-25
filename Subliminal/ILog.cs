using System;

namespace Subliminal
{
    public interface ILog<out TEntry> : IObservable<TEntry>
    {
    }
}
