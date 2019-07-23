using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Subliminal
{
    public class DerivedRateMetric : IMetric<Rate>
    {
        private readonly IMetric<Rate> _rateMetric;

        private DerivedRateMetric(IMetric<Rate> rateMetric)
        {
            _rateMetric = rateMetric;
        }

        public static DerivedRateMetric FromObservable(IObservable<long> observable)
        {
            return new DerivedRateMetric(observable
                .TimeInterval()
                .Select(count => new Rate(count.Value, count.Interval))
                .AsMetric());
        }

        public static DerivedRateMetric FromObservable(IObservable<IEnumerable<long>> observable)
        {
            return FromObservable(observable.Select(counts => counts.Sum()));
        }

        public Guid MetricId => _rateMetric.MetricId;

        public IDisposable Subscribe(IObserver<Rate> observer)
        {
            return _rateMetric.Subscribe(observer);
        }
    }
}
