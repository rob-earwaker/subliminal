using System;

namespace Subliminal
{
    public class Metric<TValue> : IMetric<TValue>
    {
        private Log<TValue> _valueLog;
        private IMetric<TValue> _derivedMetric;

        public Metric()
        {
            _valueLog = new Log<TValue>();
            _derivedMetric = _valueLog.Entries.AsMetric();
        }

        public Guid MetricId => _derivedMetric.MetricId;
        public IObservable<TValue> Values => _derivedMetric.Values;

        public void RecordValue(TValue value)
        {
            _valueLog.Append(value);
        }
    }
}
