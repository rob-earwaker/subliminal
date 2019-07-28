using System;

namespace Subliminal
{
    public class LogEntry<TValue>
    {
        public LogEntry(Guid logId, TValue value, DateTimeOffset timestamp, TimeSpan interval)
        {
            LogId = logId;
            Value = value;
            Timestamp = timestamp;
            Interval = interval;
        }
        
        public Guid LogId { get; }
        public TValue Value { get; }
        public DateTimeOffset Timestamp { get; }
        public TimeSpan Interval { get; }
    }
}
