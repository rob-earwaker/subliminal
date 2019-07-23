using System;

namespace Subliminal
{
    public class Metric<TValue> : IMetric<TValue>
    {
        private Log<TValue> _valueLog;
        private IMetric<TValue> _metric;

        public Metric()
        {
            _valueLog = new Log<TValue>();
            _metric = _valueLog.AsMetric();
        }

        public void RecordValue(TValue value)
        {
            _valueLog.Append(value);
        }

        public Guid MetricId => _metric.MetricId;

        public IDisposable Subscribe(IObserver<TValue> observer)
        {
            return _metric.Subscribe(observer);
        }
    }
}
