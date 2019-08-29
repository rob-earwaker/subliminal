using System;

namespace Subliminal
{
    /// <summary>
    /// An observable log of entries.
    /// </summary>
    public interface ILog<out TEntry> : IObservable<TEntry>
    {
    }
}
