using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    internal class ManualSource<TValue> : ISource<TValue>
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

        public IObservable<SourcedValue<TValue>> Values => _source.Values;

        public ISource<IList<SourcedValue<TValue>>> Buffer(int count, int skip)
        {
            return _source.Buffer(count, skip);
        }

        public ISource<TNewValue> Select<TNewValue>(Func<TValue, TNewValue> selector)
        {
            return _source.Select(selector);
        }
    }
}
