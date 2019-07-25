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

        public void RecordValue(TValue value)
        {
            _valueLog.Append(value);
        }

        public Guid MetricId => _derivedMetric.MetricId;

        public IDisposable Subscribe(IObserver<TValue> observer)
        {
            return _derivedMetric.Subscribe(observer);
        }
    }
}
