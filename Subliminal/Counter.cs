using System;

namespace Subliminal
{
    public class Counter : ICounter
    {
        private readonly Log<int> _log;

        public Counter()
        {
            _log = new Log<int>();
        }

        public void Increment()
        {
            IncrementBy(1);
        }

        public void IncrementBy(int increment)
        {
            if (increment <= 0)
                return;

            _log.Append(increment);
        }

        public IDisposable Subscribe(IObserver<int> observer)
        {
            return _log.Subscribe(observer);
        }
    }
}
