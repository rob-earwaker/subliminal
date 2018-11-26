using Lognostics.Events;
using System;

namespace Lognostics
{
    public class Counter
    {
        public Counter()
        {
            CounterId = Guid.NewGuid();
        }

        public Guid CounterId { get; }

        public EventHandler<CounterIncremented> Incremented;

        public void IncrementBy(int increment)
        {
            if (increment <= 0)
                throw new ArgumentException("Increment must be positive", nameof(increment));

            Incremented?.Invoke(this, new CounterIncremented(CounterId, increment));
        }
    }
}
