using System;

namespace Subliminal
{
    public class Event
    {
        public Event(Guid eventLogId, DateTimeOffset timestamp, TimeSpan interval)
        {
            EventLogId = eventLogId;
            Timestamp = timestamp;
            Interval = interval;
        }

        public static Event WithoutContext<TContext>(Event<TContext> @event)
        {
            return new Event(@event.EventLogId, @event.Timestamp, @event.Interval);
        }

        public Guid EventLogId { get; }
        public DateTimeOffset Timestamp { get; }
        public TimeSpan Interval { get; }
    }

    public class Event<TContext>
    {
        public Event(Guid eventLogId, TContext context, DateTimeOffset timestamp, TimeSpan interval)
        {
            EventLogId = eventLogId;
            Context = context;
            Timestamp = timestamp;
            Interval = interval;
        }

        public Guid EventLogId { get; }
        public TContext Context { get; }
        public DateTimeOffset Timestamp { get; }
        public TimeSpan Interval { get; }
    }
}
