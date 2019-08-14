using System;

namespace Subliminal.Sample.Api
{
    internal class EventLogQuickstart : ISample
    {
        public void RunSample()
        {
            var eventLog = new EventLog();

            eventLog.Subscribe(_ => Console.WriteLine($"Event occurred"));

            eventLog.LogOccurrence();
            // "Event occurred"
            eventLog.LogOccurrence();
            // "Event occurred"
        }
    }
}
