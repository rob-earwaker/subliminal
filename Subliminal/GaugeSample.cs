using System;

namespace Subliminal
{
    public class GaugeSample<TValue>
    {
        public GaugeSample(Guid gaugeId, TValue value, DateTimeOffset timestamp, TimeSpan interval)
        {
            GaugeId = gaugeId;
            Value = value;
            Timestamp = timestamp;
            Interval = interval;
        }

        public Guid GaugeId { get; }
        public TValue Value { get; }
        public DateTimeOffset Timestamp { get; }
        public TimeSpan Interval { get; }
    }
}
