using System;

namespace Subliminal
{
    public class Counter : ICounter
    {
        private readonly Metric<int> _metric;
        private readonly ICounter _counter;

        public Counter()
        {
            _metric = new Metric<int>();
            _counter = _metric.AsCounter();
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

        public Guid CounterId => _counter.CounterId;

        public IDisposable Subscribe(IObserver<int> observer)
        {
            return _counter.Subscribe(observer);
        }
    }
}
