using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class ManualGauge<TValue> : IGauge<TValue>
    {
        private readonly Subject<TValue> _source;
        private readonly IGauge<TValue> _gauge;

        public ManualGauge()
        {
            _source = new Subject<TValue>();
            _gauge = _source.AsObservable().AsGauge();
        }

        public IObservable<Sample<TValue>> Sampled => _gauge.Sampled;

        public void LogValue(TValue value)
        {
            _source.OnNext(value);
        }

        public IGauge<IList<Sample<TValue>>> Buffer(int count, int skip)
        {
            return _gauge.Buffer(count, skip);
        }

        public IGauge<TResult> Select<TResult>(Func<TValue, TResult> selector)
        {
            return _gauge.Select(selector);
        }
    }
}
