using System;

namespace Subliminal
{
    public class Metric<TValue> : IMetric<TValue>
    {
        private Log<TValue> _log;

        public Metric()
        {
            _log = new Log<TValue>();
        }

        public void RecordValue(TValue value)
        {
            _log.Append(value);
        }

        public void Stop()
        {
            _log.Close();
        }

        public IDisposable Subscribe(IObserver<TValue> observer)
        {
            return _log.Subscribe(observer);
        }
    }
}
