using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Subliminal.Events;

namespace Subliminal
{
    public class Gauge<TValue> : IGauge<TValue>
    {
        public Gauge(IObservable<GaugeSampled<TValue>> sampled)
        {
            Sampled = sampled.Publish().AutoConnect();
        }

        public static Gauge<TValue> FromSource(IObservable<TValue> source)
        {
            return new Gauge<TValue>(source
                .Timestamp()
                .TimeInterval()
                .Select(x => new GaugeSampled<TValue>(x.Value.Value, x.Value.Timestamp, x.Interval)));
        }

        public IObservable<GaugeSampled<TValue>> Sampled { get; }

        public IGauge<IList<TValue>> Buffer(int count, int skip)
        {
            return new Gauge<IList<TValue>>(Sampled
                .Buffer(count, skip)
                .Select(buffer => new GaugeSampled<IList<TValue>>(
                    value: buffer.Select(sampled => sampled.Value).ToList(),
                    timestamp: buffer.Last().Timestamp,
                    interval: buffer.Aggregate(TimeSpan.Zero, (interval, sampled) => interval + sampled.Interval))));
        }

        public IGauge<TResult> Select<TResult>(Func<TValue, TResult> selector)
        {
            return new Gauge<TResult>(Sampled
                .Select(sampled => new GaugeSampled<TResult>(
                    selector(sampled.Value),
                    sampled.Timestamp,
                    sampled.Interval)));
        }
    }
}
