using System;
using System.Reactive.Linq;
using Subliminal.Events;

namespace Subliminal
{
    public class Gauge<TValue> : IGauge<TValue>
    {
        public Gauge(IObservable<GaugeSampled<TValue>> sampled)
        {
            Sampled = sampled;
        }

        public IObservable<GaugeSampled<TValue>> Sampled { get; }

        public IGauge<TResult> Select<TResult>(Func<TValue, TResult> selector)
        {
            return new Gauge<TResult>(
                Sampled.Select(sampled =>
                    new GaugeSampled<TResult>(selector(sampled.Value), sampled.Timestamp, sampled.Interval)));
        }
    }
}
