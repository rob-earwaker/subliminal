using Subliminal.Events;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class Counter
    {
        private readonly Subject<CounterIncremented> _incremented;

        public Counter()
        {
            _incremented = new Subject<CounterIncremented>();
        }

        public IObservable<CounterIncremented> Incremented => _incremented.AsObservable();

        public void IncrementBy(int increment)
        {
            if (increment <= 0)
                _incremented.OnError(new ArgumentException("Increment must be positive", nameof(increment)));

            _incremented.OnNext(new CounterIncremented(increment));
        }
    }
}
