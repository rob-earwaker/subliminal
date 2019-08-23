using System;
using System.Reactive;

namespace Subliminal
{
    public interface IEvent<TEvent> : IObservable<TEvent>
    {
    }

    public interface IEvent : IEvent<Unit>
    {
    }
}
