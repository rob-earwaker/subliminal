using System;
using System.Reactive;

namespace Subliminal
{
    public interface IEvent<TContext> : IObservable<TContext>
    {
    }

    public interface IEvent : IEvent<Unit>
    {
    }
}
