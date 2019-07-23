using System;

namespace Subliminal
{
    public class Counter : ICounter
    {
        private readonly Metric<long> _incrementMetric;
        private readonly ICounter _counter;

        public Counter()
        {
            _incrementMetric = new Metric<long>();
            _counter = _incrementMetric.AsCounter();
        }

        public void Increment()
        {
            IncrementBy(1);
        }

        public void IncrementBy(long increment)
        {
            if (increment <= 0L)
                return;

            _incrementMetric.RecordValue(increment);
        }

        public Guid CounterId => _counter.CounterId;

        public IDisposable Subscribe(IObserver<long> observer)
        {
            return _counter.Subscribe(observer);
        }
    }
}
