using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Subliminal
{
    public class SampleSource<TValue> : ISampleSource<TValue>
    {
        private readonly ISource<TValue> _source;

        private SampleSource(ISource<TValue> source)
        {
            _source = source;
        }

        public static SampleSource<TValue> FromObservable(IObservable<TValue> observable)
        {
            return new SampleSource<TValue>(observable.AsSource());
        }

        public IObservable<Sample<TValue>> Samples => _source.Values.Select(Sample<TValue>.FromSourcedValue);

        public ISampleSource<IList<Sample<TValue>>> Buffer(int count, int skip)
        {
            return new SampleSource<IList<Sample<TValue>>>(_source
                .Buffer(count, skip)
                .Select<IList<Sample<TValue>>>(sourcedValues => sourcedValues
                    .Select(Sample<TValue>.FromSourcedValue)
                    .ToList()));
        }

        public ISampleSource<TNewValue> Select<TNewValue>(Func<TValue, TNewValue> selector)
        {
            return new SampleSource<TNewValue>(_source.Select(selector));
        }
    }
}
