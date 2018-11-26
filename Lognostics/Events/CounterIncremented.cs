using System;

namespace Lognostics.Events
{
    public class CounterIncremented : EventArgs
    {
        public CounterIncremented(int increment)
        {
            Increment = increment;
        }

        public int Increment { get; }
    }
}