using System;

namespace Subliminal.Sample.Api
{
    internal class QuickstartEventLog : ISample
    {
        public string SampleName => "Quickstart: EventLog";

        public void RunSample()
        {
            var messageHandled = new EventLog();

            messageHandled.Subscribe(_ => Console.WriteLine($"Message handled"));

            messageHandled.LogOccurrence();
            // "Message handled"
            messageHandled.LogOccurrence();
            // "Message handled"
        }
    }
}
