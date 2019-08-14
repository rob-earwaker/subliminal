using System;

namespace Subliminal.Sample.Api
{
    internal static class Program
    {
        private static readonly ISample[] Samples = {
            new CounterQuickstart(),
            new EventLogQuickstart(),
            new EventLogQuickstartContext(),
            new GaugeQuickstart(),
            new OperationQuickstart(),
            new OperationQuickstartContext(),
        };

        public static void Main(string[] args)
        {
            foreach (var sample in Samples)
            {
                Console.WriteLine($"=== {sample.GetType().Name} ===");
                sample.RunSample();
                Console.WriteLine();
            }
        }
    }
}
