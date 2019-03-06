using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class DerivedCounter : ICounter
    {
        private readonly IMetric<int> _metric;

        private DerivedCounter(IMetric<int> metric)
        {
            _metric = metric;
        }        

        public static DerivedCounter FromObservable(IObservable<int> observable)
        {
            return new DerivedCounter(observable.Where(increment => increment > 0).AsMetric());
        }

        public Guid CounterId => _metric.MetricId;

        public IDisposable Subscribe(IObserver<int> observer)
        {
            return _metric.Subscribe(observer);
        }
    }
}
