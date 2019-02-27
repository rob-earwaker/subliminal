using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Subliminal
{
    internal class Source<TValue> : ISource<TValue>
    {
        private Source(IObservable<SourcedValue<TValue>> values)
        {
            Values = values.Publish().AutoConnect();
        }

        public static Source<TValue> FromObservable(IObservable<TValue> observable)
        {
            return new Source<TValue>(observable
                .Timestamp()
                .TimeInterval()
                .Select(x => new SourcedValue<TValue>(x.Value.Value, x.Value.Timestamp, x.Interval)));
        }

        public IObservable<SourcedValue<TValue>> Values { get; }

        public ISource<IList<SourcedValue<TValue>>> Buffer(int count, int skip)
        {
            if (count <= 0 || skip <= 0)
                Values.Buffer(count, skip);

            return new Source<IList<SourcedValue<TValue>>>(Values
                .Buffer(Math.Max(count, skip), skip)
                .Select(buffer => new SourcedValue<IList<SourcedValue<TValue>>>(
                    value: buffer.Take(count).ToList(),
                    timestamp: buffer.Take(count).Last().Timestamp,
                    interval: buffer.Reverse().Take(skip)
                        .Aggregate(TimeSpan.Zero, (interval, sample) => interval + sample.Interval))));
        }

        public ISource<TNewValue> Select<TNewValue>(Func<TValue, TNewValue> selector)
        {
            return new Source<TNewValue>(Values
                .Select(sample => new SourcedValue<TNewValue>(
                    selector(sample.Value),
                    sample.Timestamp,
                    sample.Interval)));
        }
    }
}
