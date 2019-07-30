using System;

namespace Subliminal
{
    public class DerivedGauge<TValue> : IGauge<TValue>
    {
        private readonly IObservable<TValue> _sampled;

        private DerivedGauge(IObservable<TValue> sampled)
        {
            _sampled = sampled;
        }

        public static DerivedGauge<TValue> FromLog(ILog<TValue> log)
        {
            return new DerivedGauge<TValue>(log);
        }

        public static DerivedGauge<TValue> FromObservable(IObservable<TValue> observable)
        {
            return FromLog(observable.AsLog());
        }

        public IDisposable Subscribe(IObserver<TValue> observer)
        {
            return _sampled.Subscribe(observer);
        }
    }
}
