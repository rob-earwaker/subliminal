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
            _derivedCounter = _incrementLog.Entries.AsCounter();
        }

        public Guid CounterId => _derivedCounter.CounterId;
        public IObservable<long> Increments => _derivedCounter.Increments;
        public IObservable<RateOfChange> RateOfChange => _derivedCounter.RateOfChange;

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
