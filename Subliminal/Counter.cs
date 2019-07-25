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

        public Guid CounterId => _derivedCounter.CounterId;

        public IDisposable Subscribe(IObserver<long> observer)
        {
            return _derivedCounter.Subscribe(observer);
        }
    }
}
