using System;

namespace Subliminal.Sample.Api
{
    internal static class Program
    {
        private static readonly ISample[] Samples = {
            new QuickstartCounterTIncrement(),
            new QuickstartEvent(),
            new QuickstartEventTEvent(),
            new QuickstartEventLog(),
            new QuickstartEventLogTEvent(),
            new QuickstartLogTEntry(),
            new QuickstartGaugeTValue(),
            new QuickstartOperation(),
            new QuickstartOperationTContext(),
        };

        public static void Main(string[] args)
        {
            foreach (var sample in Samples)
            {
                Console.WriteLine($"=== {sample.SampleName} ===");
                sample.RunSample();
                Console.WriteLine();
            }
        }
    }
}
