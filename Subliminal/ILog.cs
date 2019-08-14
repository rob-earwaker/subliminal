using System;

namespace Subliminal
{
    internal interface ILog<TEntry> : IObservable<TEntry>
    {
    }
}
