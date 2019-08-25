using System;
using System.Reactive;

namespace Subliminal
{
    public interface IEvent<out TEvent> : IObservable<TEvent>
    {
    }

    public interface IEvent : IEvent<Unit>
    {
    }
}
