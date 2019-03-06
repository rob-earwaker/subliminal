using System;

namespace Subliminal
{
    public class DerivedMetric<TValue> : IMetric<TValue>
    {
        private readonly ILog<TValue> _log;

        private DerivedMetric(ILog<TValue> log)
        {
            _log = log;
        }

        public static DerivedMetric<TValue> FromLog(ILog<TValue> log)
        {
            return new DerivedMetric<TValue>(log);
        }

        public static DerivedMetric<TValue> FromObservable(IObservable<TValue> observable)
        {
            return FromLog(observable.AsLog());
        }

        public Guid MetricId => _log.LogId;

        public IDisposable Subscribe(IObserver<TValue> observer)
        {
            return _log.Subscribe(observer);
        }
    }
}
