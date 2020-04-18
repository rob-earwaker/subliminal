using System;

namespace Subliminal.Sample.Api
{
    internal static class Program
    {
        private static readonly ISample[] Samples = {
            new QuickstartCounter(),
            new QuickstartEvent(),
            new QuickstartEventTEvent(),
            new QuickstartEventLog(),
            new QuickstartEventLogTEvent(),
            new QuickstartLogTEntry(),
            new QuickstartGauge(),
            new QuickstartOperationLog(),
            new QuickstartOperationLogTContext(),
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
