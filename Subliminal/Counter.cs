using System;

namespace Subliminal
{
    public class Counter : ICounter
    {
        private readonly Metric<int> _metric;

        public Counter()
        {
            _metric = new Metric<int>();
        }

        public void Increment()
        {
            IncrementBy(1);
        }

        public void IncrementBy(int increment)
        {
            if (increment <= 0)
                return;

            _metric.RecordValue(increment);
        }

        public void Stop()
        {
            _metric.Stop();
        }

        public IDisposable Subscribe(IObserver<int> observer)
        {
            return _metric.Subscribe(observer);
        }
    }
}
