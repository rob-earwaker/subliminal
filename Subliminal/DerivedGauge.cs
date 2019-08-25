using System;

namespace Subliminal
{
    public sealed class DerivedGauge<TValue> : IGauge<TValue>
    {
        private readonly ILog<TValue> _valueLog;

        private DerivedGauge(ILog<TValue> valueLog)
        {
            _valueLog = valueLog;
        }

        public static DerivedGauge<TValue> FromObservable(IObservable<TValue> observable)
        {
            return new DerivedGauge<TValue>(observable.AsLog());
        }

        public IDisposable Subscribe(IObserver<TValue> observer)
        {
            return _valueLog.Subscribe(observer);
        }
    }
}
