using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class Counter
    {
        private readonly Subject<int> _subject;

        public Counter()
        {
            _subject = new Subject<int>();
        }

        public IObservable<int> Incremented => _subject.AsObservable();

        public void IncrementBy(int increment)
        {
            if (increment <= 0)
                _subject.OnError(new ArgumentException("Increment must be positive", nameof(increment)));

            _subject.OnNext(increment);
        }
    }
}
