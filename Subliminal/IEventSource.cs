using System;

namespace Subliminal
{
    public interface IEventSource<TEvent>
    {
        IObservable<Event<TEvent>> Source { get; }
    }
}
