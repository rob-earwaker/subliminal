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

        public Guid GaugeId => _derivedGauge.GaugeId;
        public IObservable<GaugeSample<TValue>> Sampled => _derivedGauge.Sampled;

        public void RecordValue(TValue value)
        {
            _valueLog.Append(value);
        }
    }
}
