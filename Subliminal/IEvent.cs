using System;

namespace Subliminal
{
    internal interface IEvent<TEvent> : IObservable<TEvent>
    {
    }
}
