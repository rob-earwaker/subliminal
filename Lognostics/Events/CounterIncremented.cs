using System;

namespace Lognostics.Events
{
    public class CounterIncremented : EventArgs
    {
        public CounterIncremented(Guid counterId, int increment)
        {
            CounterId = counterId;
            Increment = increment;
        }

        public Guid CounterId { get; }
        public int Increment { get; }
    }
}