using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class DerivedCounter : ICounter
    {
        private readonly IMetric<long> _incrementMetric;

        private DerivedCounter(IMetric<long> incrementMetric)
        {
            _incrementMetric = incrementMetric;
        }

        public static DerivedCounter FromObservable(IObservable<long> observable)
        {
            return new DerivedCounter(observable.Where(increment => increment > 0L).AsMetric());
        }

        public Guid CounterId => _incrementMetric.MetricId;

        public IDisposable Subscribe(IObserver<long> observer)
        {
            return _incrementMetric.Subscribe(observer);
        }
    }
}
