using System;
using System.IO;

namespace Subliminal.Sample.Api
{
    internal class QuickstartEventTEvent : ISample
    {
        public string SampleName => "Quickstart: Event<TEvent>";

        public void RunSample()
        {
            var fileWritten = new Event<string>();

            fileWritten.Subscribe(fileName =>
                Console.WriteLine($"Written file '{fileName}'"));

            using (var fileStream = File.OpenWrite(Path.GetTempFileName()))
            {
                // ...

                fileWritten.Raise(fileStream.Name);
                // "Written file '...\Temp\tmpF9E.tmp'"
            }

            fileWritten.Subscribe(fileName =>
                Console.WriteLine($"Already written file '{fileName}'"));
            // "Already written file '...\Temp\tmpF9E.tmp'"
        }
    }
}
