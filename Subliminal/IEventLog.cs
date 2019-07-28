using System;

namespace Subliminal
{
    public interface IEventLog
    {
        Guid EventLogId { get; }
        IObservable<Event> EventLogged { get; }
        ICounter EventCounter { get; }
    }

    public interface IEventLog<TContext>
    {
        Guid EventLogId { get; }
        IObservable<Event<TContext>> EventLogged { get; }
        ICounter EventCounter { get; }
    }
}
