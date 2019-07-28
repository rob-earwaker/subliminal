using System;

namespace Subliminal
{
    public class Counter : ICounter
    {
        private readonly Log<long> _incrementLog;
        private readonly ICounter _derivedCounter;

        public Counter()
        {
            _incrementLog = new Log<long>();
            _derivedCounter = _incrementLog.AsCounter();
        }

        public Guid CounterId => _derivedCounter.CounterId;
        public IObservable<CounterIncrement> Incremented => _derivedCounter.Incremented;

        public void Increment()
        {
            IncrementBy(1L);
        }

        public void IncrementBy(long increment)
        {
            if (increment <= 0L)
                return;

            _incrementLog.Append(increment);
        }
    }
}
