using System;
using System.IO;

namespace Subliminal.Sample.Api
{
    internal class QuickstartEvent : ISample
    {
        public string SampleName => "Quickstart: Event";

        public void RunSample()
        {
            var streamDisposed = new Event();

            streamDisposed.Subscribe(_ => Console.WriteLine("Stream disposed"));

            using (var stream = new MemoryStream())
            {
                // ...
            }

            streamDisposed.Raise();
            // "Stream disposed"

            streamDisposed.Subscribe(_ => Console.WriteLine("Stream still disposed"));
            // "Stream still disposed"
        }
    }
}
