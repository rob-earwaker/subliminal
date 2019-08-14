using System;
using System.Threading;

namespace Subliminal.Sample.Api
{
    internal class QuickstartOperation : ISample
    {
        public string SampleName => "Quickstart: Operation";

        public void RunSample()
        {
            var addItemToBasket = new Operation();

            addItemToBasket.Started.Subscribe(started =>
                Console.WriteLine("Started adding item to basket"));

            addItemToBasket.Completed.Subscribe(completed =>
                Console.WriteLine($"Successfully added item to basket in {completed.Duration}"));

            addItemToBasket.Canceled.Subscribe(canceled =>
                Console.WriteLine($"Canceled adding item to basket after {canceled.Duration}"));

            addItemToBasket.Ended.Subscribe(ended =>
                Console.WriteLine($"Finished adding item to basket after {ended.Duration}"));
            
            using (var timer = addItemToBasket.StartNewTimer())
            // "Started adding item to basket"
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            // "Successfully added item to basket in 00:00:01.0008729"
            // "Finished adding item to basket after 00:00:01.0008729"

            using (var timer = addItemToBasket.StartNewTimer())
            // "Started adding item to basket"
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                timer.Complete();
                // "Successfully added item to basket in 00:00:01.0006293"
                // "Finished adding item to basket after 00:00:01.0006293"
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            using (var timer = addItemToBasket.StartNewTimer())
            // "Started adding item to basket"
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                timer.Cancel();
                // "Canceled adding item to basket after 00:00:01.0006214"
                // "Finished adding item to basket after 00:00:01.0006214"
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }
    }
}
