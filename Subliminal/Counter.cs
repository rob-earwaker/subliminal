using System;

namespace Subliminal
{
    public class Counter<TIncrement> : ICounter<TIncrement>
    {
        private readonly Log<TIncrement> _incrementLog;
        private readonly ICounter<TIncrement> _derivedCounter;

        public Counter()
        {
            _incrementLog = new Log<TIncrement>();
            _derivedCounter = _incrementLog.AsCounter();
        }

        public void IncrementBy(TIncrement increment)
        {
            _incrementLog.Append(increment);
        }

        public IDisposable Subscribe(IObserver<TIncrement> observer)
        {
            return _derivedCounter.Subscribe(observer);
        }
    }
}
