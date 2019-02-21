using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class Gauge<TGauge>
    {
        private readonly Subject<TGauge> _sampled;

        public Gauge()
        {
            _sampled = new Subject<TGauge>();
        }

        public IObservable<TGauge> Sampled => _sampled.AsObservable();

        public void LogValue(TGauge value)
        {
            _sampled.OnNext(value);
        }
    }
}
