using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Subliminal
{
    public class Source<TValue> : ISource<TValue>
    {
        private Source(IObservable<Sourced<TValue>> values)
        {
            Values = values.Publish().AutoConnect();
        }

        public static Source<TValue> FromObservable(IObservable<TValue> source)
        {
            return new Source<TValue>(source
                .Timestamp()
                .TimeInterval()
                .Select(x => new Sourced<TValue>(x.Value.Value, x.Value.Timestamp, x.Interval)));
        }

        public IObservable<Sourced<TValue>> Values { get; }

        public ISource<IList<Sourced<TValue>>> Buffer(int count, int skip)
        {
            if (count <= 0 || skip <= 0)
                Values.Buffer(count, skip);

            return new Source<IList<Sourced<TValue>>>(Values
                .Buffer(Math.Max(count, skip), skip)
                .Select(buffer => new Sourced<IList<Sourced<TValue>>>(
                    value: buffer.Take(count).ToList(),
                    timestamp: buffer.Take(count).Last().Timestamp,
                    interval: buffer.Reverse().Take(skip)
                        .Aggregate(TimeSpan.Zero, (interval, sample) => interval + sample.Interval))));
        }

        public ISource<TNewValue> Select<TNewValue>(Func<TValue, TNewValue> selector)
        {
            return new Source<TNewValue>(Values
                .Select(sample => new Sourced<TNewValue>(
                    selector(sample.Value),
                    sample.Timestamp,
                    sample.Interval)));
        }
    }
}
