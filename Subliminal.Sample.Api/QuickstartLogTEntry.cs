using System;

namespace Subliminal.Sample.Api
{
    internal class QuickstartLogTEntry : ISample
    {
        public string SampleName => "Quickstart: Log<TEntry>";

        public void RunSample()
        {
            var messageLog = new Log<string>();

            messageLog.Subscribe(message =>
                Console.WriteLine($"Received message '{message}'"));

            messageLog.Append("hello");
            // "Received message 'hello'"

            messageLog.Append("world");
            // "Received message 'world'"
        }
    }
}
