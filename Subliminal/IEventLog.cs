using System;
using System.Reactive;

namespace Subliminal
{
    public interface IEventLog<TEvent> : IObservable<TEvent>
    {
    }

    public interface IEventLog : IEventLog<Unit>
    {
    }
}
