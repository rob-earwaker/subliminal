using System;

namespace Subliminal
{
    public class DerivedMetric<TValue> : IMetric<TValue>
    {
        private readonly ILog<TValue> _valueLog;

        private DerivedMetric(ILog<TValue> valueLog)
        {
            _valueLog = valueLog;
        }

        public static DerivedMetric<TValue> FromLog(ILog<TValue> log)
        {
            return new DerivedMetric<TValue>(log);
        }

        public static DerivedMetric<TValue> FromObservable(IObservable<TValue> observable)
        {
            return FromLog(observable.AsLog());
        }

        public Guid MetricId => _valueLog.LogId;

        public IDisposable Subscribe(IObserver<TValue> observer)
        {
            return _valueLog.Subscribe(observer);
        }
    }
}
