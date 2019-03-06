using System;

namespace Subliminal
{
    public class Metric<TValue> : IMetric<TValue>
    {
        private Log<TValue> _log;
        private IMetric<TValue> _metric;

        public Metric()
        {
            _log = new Log<TValue>();
            _metric = _log.AsMetric();
        }

        public void RecordValue(TValue value)
        {
            _log.Append(value);
        }

        public Guid MetricId => _metric.MetricId;

        public IDisposable Subscribe(IObserver<TValue> observer)
        {
            return _metric.Subscribe(observer);
        }
    }
}
