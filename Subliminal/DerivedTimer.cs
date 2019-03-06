using System;

namespace Subliminal
{
    public class DerivedTimer : ITimer
    {
        private readonly IMetric<TimeSpan> _metric;

        private DerivedTimer(IMetric<TimeSpan> metric)
        {
            _metric = metric;
        }

        public static DerivedTimer FromObservable(IObservable<TimeSpan> observable)
        {
            return new DerivedTimer(observable.AsMetric());
        }

        public Guid TimerId => _metric.MetricId;

        public IDisposable Subscribe(IObserver<TimeSpan> observer)
        {
            return _metric.Subscribe(observer);
        }
    }
}
