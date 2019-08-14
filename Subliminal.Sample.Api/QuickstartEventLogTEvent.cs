using System;

namespace Subliminal.Sample.Api
{
    internal class QuickstartEventLogTEvent : ISample
    {
        public string SampleName => "Quickstart: EventLog<TEvent>";

        public class FileDeleted
        {
            public string FileName { get; set; }
        }

        public void RunSample()
        {
            var eventLog = new EventLog<FileDeleted>();

            eventLog.Subscribe(context =>
                Console.WriteLine($"File '{context.FileName}' was deleted"));

            eventLog.LogOccurrence(new FileDeleted { FileName = "temp01.txt" });
            // "File 'temp01.txt' was deleted"
            eventLog.LogOccurrence(new FileDeleted { FileName = "temp02.txt" });
            // "File 'temp02.txt' was deleted"
        }
    }
}
