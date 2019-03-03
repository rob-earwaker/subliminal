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

        public static DerivedMetric<TValue> FromObservable(IObservable<TValue> observable)
        {
            return new DerivedMetric<TValue>(observable.AsLog());
        }

        public IDisposable Subscribe(IObserver<TValue> observer)
        {
            return _log.Subscribe(observer);
        }
    }
}
