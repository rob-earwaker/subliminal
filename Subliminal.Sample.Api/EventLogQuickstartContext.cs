using System;

namespace Subliminal.Sample.Api
{
    internal class EventLogQuickstartContext : ISample
    {
        public class Context
        {
            public string Message { get; set; }
        }

        public void RunSample()
        {
            var eventLog = new EventLog<Context>();

            eventLog.Subscribe(context =>
                Console.WriteLine($"Message '{context.Message}' was sent"));

            eventLog.LogOccurrence(new Context { Message = "hello" });
            // "Message 'hello' was sent"
            eventLog.LogOccurrence(new Context { Message = "world" });
            // "Message 'world' was sent"
        }
    }
}
