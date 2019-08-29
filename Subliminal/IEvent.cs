using System;
using System.Reactive;

namespace Subliminal
{
    /// <summary>
    /// An observable event that emits a single value and then completes.
    /// </summary>
    public interface IEvent<out TEvent> : IObservable<TEvent>
    {
    }

    /// <summary>
    /// An observable event that emits a single value and then completes.
    /// </summary>
    public interface IEvent : IEvent<Unit>
    {
    }
}
