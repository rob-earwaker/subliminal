using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Subliminal
{
    internal class Source<TValue> : ISource<TValue>
    {
        private Source(IObservable<Observation<TValue>> observations)
        {
            Observations = observations;
        }

        public static Source<TValue> FromObservable(IObservable<TValue> observable)
        {
            return new Source<TValue>(observable
                .Timestamp()
                .TimeInterval()
                .Select(x => new Observation<TValue>(x.Value.Value, x.Value.Timestamp, x.Interval))
                .Publish()
                .AutoConnect());
        }

        public IObservable<Observation<TValue>> Observations { get; }

        public ISource<IList<Observation<TValue>>> Buffer(int count, int skip)
        {
            if (count <= 0 || skip <= 0)
                Observations.Buffer(count, skip);

            return new Source<IList<Observation<TValue>>>(Observations
                .Buffer(Math.Max(count, skip), skip)
                .Select(buffer => new Observation<IList<Observation<TValue>>>(
                    value: buffer.Take(count).ToList(),
                    timestamp: buffer.Take(count).Last().Timestamp,
                    interval: buffer.Reverse().Take(skip)
                        .Aggregate(TimeSpan.Zero, (interval, sample) => interval + sample.Interval))));
        }

        public ISource<TNewValue> Select<TNewValue>(Func<TValue, TNewValue> selector)
        {
            return new Source<TNewValue>(Observations
                .Select(observation => new Observation<TNewValue>(
                    selector(observation.Value),
                    observation.Timestamp,
                    observation.Interval)));
        }
    }
}
