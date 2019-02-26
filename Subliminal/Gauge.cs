using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Subliminal
{
    public class Gauge<TValue> : IGauge<TValue>
    {
        private Gauge(IObservable<Sample<TValue>> source)
        {
            Sampled = source.Publish().AutoConnect();
        }

        public static Gauge<TValue> FromSource(IObservable<TValue> source)
        {
            return new Gauge<TValue>(source
                .Timestamp()
                .TimeInterval()
                .Select(x => new Sample<TValue>(x.Value.Value, x.Value.Timestamp, x.Interval)));
        }

        public IObservable<Sample<TValue>> Sampled { get; }

        public IGauge<IList<Sample<TValue>>> Buffer(int count, int skip)
        {
            if (count <= 0 || skip <= 0)
                Sampled.Buffer(count, skip);
            
            return new Gauge<IList<Sample<TValue>>>(Sampled
                .Buffer(Math.Max(count, skip), skip)
                .Select(buffer => new Sample<IList<Sample<TValue>>>(
                    value: buffer.Take(count).ToList(),
                    timestamp: buffer.Take(count).Last().Timestamp,
                    interval: buffer.Reverse().Take(skip)
                        .Aggregate(TimeSpan.Zero, (interval, sample) => interval + sample.Interval))));
        }

        public IGauge<TResult> Select<TResult>(Func<TValue, TResult> selector)
        {
            return new Gauge<TResult>(Sampled
                .Select(sample => new Sample<TResult>(
                    selector(sample.Value),
                    sample.Timestamp,
                    sample.Interval)));
        }
    }
}
