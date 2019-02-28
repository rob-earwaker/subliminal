using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class Sink<TValue> : ISink<TValue>
    {
        private readonly ISubject<TValue> _subject;

        public Sink()
        {
            _subject = new Subject<TValue>();
        }

        public ISource<TValue> AsSource()
        {
            return _subject.AsObservable().AsSource();
        }

        public void OnCompleted()
        {
            _subject.OnCompleted();
        }

        public void OnError(Exception error)
        {
            _subject.OnError(error);
        }

        public void OnNext(TValue value)
        {
            _subject.OnNext(value);
        }
    }
}
