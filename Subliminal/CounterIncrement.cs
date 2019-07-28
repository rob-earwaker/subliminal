using System;

namespace Subliminal
{
    public class CounterIncrement
    {
        public CounterIncrement(Guid counterId, long value, DateTimeOffset timestamp, TimeSpan interval)
        {
            CounterId = counterId;
            Value = value;
            Timestamp = timestamp;
            Interval = interval;
        }

        public Guid CounterId { get; }
        public long Value { get; }
        public DateTimeOffset Timestamp { get; }
        public TimeSpan Interval { get; }
    }
}
