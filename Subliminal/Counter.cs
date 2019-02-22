using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class Counter
    {
        private readonly Subject<int> _incremented;

        public Counter()
        {
            _incremented = new Subject<int>();
        }

        public IObservable<int> Incremented => _incremented.AsObservable();

        public void IncrementBy(int increment)
        {
            if (increment <= 0)
                _incremented.OnError(new ArgumentException("Increment must be positive", nameof(increment)));

            _incremented.OnNext(increment);
        }
    }
}
