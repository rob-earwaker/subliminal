using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class DerivedMetric<TValue> : IMetric<TValue>
    {
        private readonly IObservable<TValue> _source;

        internal DerivedMetric(IObservable<TValue> source)
        {
            _source = source;
        }

        public static DerivedMetric<TValue> FromObservable(IObservable<TValue> observable)
        {
            return new DerivedMetric<TValue>(observable.Publish().AutoConnect());
        }

        public IDisposable Subscribe(IObserver<TValue> observer)
        {
            return _source.Subscribe(observer);
        }
    }
}
