using System;

namespace Subliminal.Sample.Api
{
    internal class CounterQuickstart : ISample
    {
        public void RunSample()
        {
            var counter = new Counter<long>();

            counter.Subscribe(increment =>
                Console.WriteLine($"Counter incremented by {increment}"));

            counter.IncrementBy(2);
            // "Counter incremented by 2"
            counter.IncrementBy(125);
            // "Counter incremented by 125"
        }
    }
}
