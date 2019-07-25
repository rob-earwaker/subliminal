using System;

namespace Subliminal
{
    public interface IEventLog<TEvent>
    {
        Guid EventLogId { get; }
        IObservable<TEvent> Events { get; }
        ICounter EventCounter { get; }
    }
}
