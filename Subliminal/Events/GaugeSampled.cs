using System;

namespace Subliminal.Events
{
    public class GaugeSampled<TValue>
    {
        public GaugeSampled(TValue value, DateTimeOffset timestamp, TimeSpan interval)
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
