using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class Metric<TValue> : IMetric<TValue>
    {
        private readonly Subject<TValue> _subject;

        public Metric()
        {
            _subject = new Subject<TValue>();
        }

        public void RecordValue(TValue value)
        {
            _subject.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<TValue> observer)
        {
            return _subject.Subscribe(observer);
        }
    }
}
