using System;

namespace Subliminal
{
    public class Observation<TValue>
    {
        public Observation(TValue observedValue, DateTimeOffset timestamp, TimeSpan interval)
        {
            ObservedValue = observedValue;
            Timestamp = timestamp;
            Interval = interval;
        }

        public TValue ObservedValue { get; }
        public DateTimeOffset Timestamp { get; }
        public TimeSpan Interval { get; }
    }
}
