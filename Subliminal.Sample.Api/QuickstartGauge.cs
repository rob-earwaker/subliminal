using System;
using System.Collections.Generic;

namespace Subliminal.Sample.Api
{
    internal class QuickstartGauge : ISample
    {
        public string SampleName => "Quickstart: Gauge";

        public void RunSample()
        {
            var queueSize = new Gauge();

            queueSize.Subscribe(value =>
                Console.WriteLine($"Current queue size is {value}"));

            var queue = new Queue<object>();
            queue.Enqueue(new object());
            queue.Enqueue(new object());
            queue.Enqueue(new object());

            queueSize.LogValue(queue.Count);
            // "Current queue size is 3"

            queue.Dequeue();

            queueSize.LogValue(queue.Count);
            // "Current queue size is 2"
        }
    }
}
