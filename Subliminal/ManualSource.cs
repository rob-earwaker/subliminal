using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class ManualSource<TValue> : ISource<TValue>
    {
        private readonly Subject<TValue> _subject;
        private readonly ISource<TValue> _source;

        public ManualSource()
        {
            _subject = new Subject<TValue>();
            _source = _subject.AsObservable().AsSource();
        }

        public void LogOccurrence(TValue value)
        {
            _subject.OnNext(value);
        }

        public IObservable<Sourced<TValue>> Values => _source.Values;

        public ISource<IList<Sourced<TValue>>> Buffer(int count, int skip)
        {
            return _source.Buffer(count, skip);
        }

        public ISource<TNewValue> Select<TNewValue>(Func<TValue, TNewValue> selector)
        {
            return _source.Select(selector);
        }
    }
}
