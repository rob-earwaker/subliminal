using System;

namespace Subliminal
{
    public class Gauge<TValue> : IGauge<TValue>
    {
        private Log<TValue> _valueLog;
        private IGauge<TValue> _derivedGauge;

        public Gauge()
        {
            _valueLog = new Log<TValue>();
            _derivedGauge = _valueLog.AsGauge();
        }

        public void LogValue(TValue value)
        {
            _valueLog.Append(value);
        }

        public IDisposable Subscribe(IObserver<TValue> observer)
        {
            return _derivedGauge.Subscribe(observer);
        }
    }
}
