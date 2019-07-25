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

        public static DerivedMetric<TValue> FromLog(ILog<TValue> valueLog)
        {
            return new DerivedMetric<TValue>(valueLog);
        }

        public static DerivedMetric<TValue> FromObservable(IObservable<TValue> values)
        {
            return FromLog(values.AsLog());
        }

        public Guid MetricId => _valueLog.LogId;

        public IDisposable Subscribe(IObserver<TValue> observer)
        {
            return _valueLog.Entries.Subscribe(observer);
        }
    }
}
