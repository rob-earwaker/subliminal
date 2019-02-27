using System;

namespace Subliminal
{
    public class Event<TEvent>
    {
        public Event(TEvent value, DateTimeOffset timestamp, TimeSpan interval)
        {
            Value = value;
            Timestamp = timestamp;
            Interval = interval;
        }

        public TEvent Value { get; }
        public DateTimeOffset Timestamp { get; }
        public TimeSpan Interval { get; }
    }
}
