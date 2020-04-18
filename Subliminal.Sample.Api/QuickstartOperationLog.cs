using System;
using System.Threading;

namespace Subliminal.Sample.Api
{
    internal class QuickstartOperationLog : ISample
    {
        public string SampleName => "Quickstart: OperationLog";

        public void RunSample()
        {
            var addToBasket = new OperationLog();

            addToBasket.Started.Subscribe(started =>
                Console.WriteLine($"Started AddToBasket operation {started.OperationId}"));

            addToBasket.Completed.Subscribe(completed =>
                Console.WriteLine($"AddToBasket operation {completed.OperationId} took {completed.Duration}"));

            addToBasket.Canceled.Subscribe(canceled =>
                Console.WriteLine($"AddToBasket operation {canceled.OperationId} was canceled"));

            using (var timer = addToBasket.StartNewTimer())
            // "Started AddToBasket operation qRWENlQa"
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            // "AddToBasket operation qRWENlQa took 00:00:01.0012713"

            using (var timer = addToBasket.StartNewTimer())
            // "Started AddToBasket operation FnUHNTBp"
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                timer.Stop();
                // "AddToBasket operation FnUHNTBp took 00:00:01.0006734"
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            using (var timer = addToBasket.StartNewTimer())
            // "Started AddToBasket operation pbM7EBz4"
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                timer.Cancel();
                // "AddToBasket operation pbM7EBz4 was canceled"
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }
    }
}
