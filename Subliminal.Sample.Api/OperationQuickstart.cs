using System;
using System.Threading;

namespace Subliminal.Sample.Api
{
    internal class OperationQuickstart : ISample
    {
        public void RunSample()
        {
            var operation = new Operation();

            operation.Started.Subscribe(started =>
                Console.WriteLine("Operation started"));

            operation.Completed.Subscribe(completed =>
                Console.WriteLine($"Operation was completed after {completed.Duration}"));

            operation.Canceled.Subscribe(canceled =>
                Console.WriteLine($"Operation was canceled after {canceled.Duration}"));

            operation.Ended.Subscribe(ended =>
                Console.WriteLine($"Operation was ended after {ended.Duration}"));

            using (var timer = operation.StartNewTimer())
            // "Operation started"
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            // "Operation was completed after 00:00:01.0943984"
            // "Operation was ended after 00:00:01.0943984"

            using (var timer = operation.StartNewTimer())
            // "Operation started"
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                timer.Complete();
                // "Operation was completed after 00:00:01.0908245"
                // "Operation was ended after 00:00:01.0908245"
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            using (var timer = operation.StartNewTimer())
            // "Operation started"
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                timer.Cancel();
                // "Operation was canceled after 00:00:01.0952762"
                // "Operation was ended after 00:00:01.0952762"
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }
    }
}
