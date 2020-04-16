using System;

namespace Subliminal
{
    /// <summary>
    /// A gauge that both captures and emits values.
    /// </summary>
    public sealed class Gauge : IGauge
    {
        private Log<double> _valueLog;
        private IGauge _derivedGauge;

        /// <summary>
        /// Creates a gauge that both captures and emits values.
        /// </summary>
        public Gauge()
        {
            _valueLog = new Log<double>();
            _derivedGauge = _valueLog.AsGauge();
        }

        /// <summary>
        /// Captures a value and emits it to all observers.
        /// </summary>
        public void LogValue(double value)
        {
            _valueLog.Append(value);
        }

        /// <summary>
        /// Subscribes an observer such that it receives all future values emitted
        /// by the gauge. The returned <see cref="IDisposable" /> can be used to
        /// cancel this subscription.
        /// </summary>
        public IDisposable Subscribe(IObserver<double> observer)
        {
            return _derivedGauge.Subscribe(observer);
        }
    }
}
