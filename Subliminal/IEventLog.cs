using System;

namespace Subliminal
{
    public interface IEventLog<TEvent> : IObservable<TEvent>
    {
        Guid EventLogId { get; }
        ICounter EventCounter { get; }
    }
}
