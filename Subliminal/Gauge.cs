using Subliminal.Events;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class Gauge<TValue>
    {
        private readonly Subject<GaugeSampled<TValue>> _sampled;

        public Gauge()
        {
            _sampled = new Subject<GaugeSampled<TValue>>();
        }

        public IObservable<GaugeSampled<TValue>> Sampled => _sampled.AsObservable();

        public void LogValue(TValue value)
        {
            _sampled.OnNext(new GaugeSampled<TValue>(value));
        }
    }
}
