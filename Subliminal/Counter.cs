using Subliminal.Events;
using System;

namespace Subliminal
{
    public class Counter
    {
        public event EventHandler<CounterIncremented> Incremented;

        public void IncrementBy(int increment)
        {
            if (increment <= 0)
                throw new ArgumentException("Increment must be positive", nameof(increment));

            Incremented?.Invoke(this, new CounterIncremented(increment));
        }
    }
}
