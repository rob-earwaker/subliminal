using System;
using System.Reactive;

namespace Subliminal
{
    public interface IEventLog<TContext> : IObservable<TContext>
    {
    }

    public interface IEventLog : IEventLog<Unit>
    {
    }
}
