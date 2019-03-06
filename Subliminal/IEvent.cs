using System;

namespace Subliminal
{
    public interface IEvent<TEvent> : IObservable<TEvent>
    {
        Guid EventId { get; }
    }
}
