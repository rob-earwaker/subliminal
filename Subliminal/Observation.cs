using System;

namespace Subliminal
{
    public class Observation<TValue>
    {
        public Observation(TValue value, DateTimeOffset timestamp, TimeSpan interval)
        {
            Value = value;
            Timestamp = timestamp;
            Interval = interval;
        }

        public TValue Value { get; }
        public DateTimeOffset Timestamp { get; }
        public TimeSpan Interval { get; }
    }
}
