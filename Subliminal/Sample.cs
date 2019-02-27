using System;

namespace Subliminal
{
    public class Sample<TValue>
    {
        public Sample(TValue value, DateTimeOffset timestamp, TimeSpan interval)
        {
            Value = value;
            Timestamp = timestamp;
            Interval = interval;
        }

        internal static Sample<TValue> FromSourcedValue(SourcedValue<TValue> sourcedValue)
        {
            return new Sample<TValue>(
                sourcedValue.Value,
                sourcedValue.Timestamp,
                sourcedValue.Interval);
        }

        public TValue Value { get; }
        public DateTimeOffset Timestamp { get; }
        public TimeSpan Interval { get; }
    }
}
