using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class DerivedEventLog : IEventLog
    {
        private DerivedEventLog(Guid eventLogId, IObservable<Event> eventLogged, ICounter eventCounter)
        {
            EventLogId = eventLogId;
            EventLogged = eventLogged;
            EventCounter = eventCounter;
        }

        private static DerivedEventLog FromEventLog<TContext>(IEventLog<TContext> eventLog)
        {
            return new DerivedEventLog(
                eventLog.EventLogId,
                eventLog.EventLogged.Select(Event.WithoutContext),
                eventLog.EventCounter);
        }

        public static DerivedEventLog FromLog<TContext>(ILog<TContext> occurrenceLog)
        {
            return FromEventLog(occurrenceLog.AsEventLog());
        }

        public static DerivedEventLog FromObservable<TContext>(IObservable<TContext> occurrences)
        {
            return FromLog(occurrences.AsLog());
        }

        public Guid EventLogId { get; }
        public IObservable<Event> EventLogged { get; }
        public ICounter EventCounter { get; }
    }

    public class DerivedEventLog<TContext> : IEventLog<TContext>
    {
        private DerivedEventLog(Guid eventLogId, IObservable<Event<TContext>> eventLogged, ICounter eventCounter)
        {
            EventLogId = eventLogId;
            EventLogged = eventLogged;
            EventCounter = eventCounter;
        }

        public static DerivedEventLog<TContext> FromLog(ILog<TContext> contextLog)
        {
            var eventLogId = Guid.NewGuid();

            var eventLogged = contextLog.EntryLogged
                .Select(entry => new Event<TContext>(eventLogId, entry.Value, entry.Timestamp, entry.Interval));

            var eventCounter = eventLogged.Select(_ => 1L).AsCounter();

            return new DerivedEventLog<TContext>(eventLogId, eventLogged, eventCounter);
        }

        public static DerivedEventLog<TContext> FromObservable(IObservable<TContext> context)
        {
            return FromLog(context.AsLog());
        }

        public Guid EventLogId { get; }
        public IObservable<Event<TContext>> EventLogged { get; }
        public ICounter EventCounter { get; }
    }
}
