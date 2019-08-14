using System;
using System.Threading;

namespace Subliminal.Sample.Api
{
    internal class OperationQuickstartContext : ISample
    {
        public class Context
        {
            public string Message { get; set; }
        }

        public void RunSample()
        {
            var operation = new Operation<Context>();

            operation.Started.Subscribe(started =>
                Console.WriteLine($"Operation started with message '{started.Context.Message}'"));

            operation.Ended.Subscribe(ended =>
                Console.WriteLine($"Operation ended with message '{ended.Context.Message}'"));

            using (var timer = operation.StartNewTimer(new Context { Message = "hello" }))
            // "Operation started with message 'hello'"
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            // "Operation ended with message 'hello'"

            using (var timer = operation.StartNewTimer(new Context { Message = "world" }))
            // "Operation started with message 'world'"
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            // "Operation ended with message 'world'"
        }
    }
}
