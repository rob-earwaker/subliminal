using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class ManualSampleSource<TValue> : ISampleSource<TValue>
    {
        private readonly Subject<TValue> _subject;
        private readonly ISampleSource<TValue> _sampleSource;

        public ManualSampleSource()
        {
            _subject = new Subject<TValue>();
            _sampleSource = _subject.AsObservable().AsSampleSource();
        }

        public void OnNext(TValue value)
        {
            _subject.OnNext(value);
        }

        public IObservable<Sample<TValue>> Samples => _sampleSource.Samples;

        public ISampleSource<IList<Sample<TValue>>> Buffer(int count, int skip)
        {
            return _sampleSource.Buffer(count, skip);
        }

        public ISampleSource<TResult> Select<TResult>(Func<TValue, TResult> selector)
        {
            return _sampleSource.Select(selector);
        }
    }
}
