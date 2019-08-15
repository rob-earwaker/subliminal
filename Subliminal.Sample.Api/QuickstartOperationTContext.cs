using System;
using System.Threading;

namespace Subliminal.Sample.Api
{
    internal class QuickstartOperationTContext : ISample
    {
        public string SampleName => "Quickstart: Operation<TContext>";

        public class RegisterUser
        {
            public string Name { get; set; }
        }

        public void RunSample()
        {
            var registerUser = new Operation<RegisterUser>();

            registerUser.Started.Subscribe(started =>
                Console.WriteLine($"Started RegisterUser operation for {started.Context.Name}"));

            registerUser.Completed.Subscribe(completed =>
                Console.WriteLine(
                    $"RegisterUser operation for {completed.Context.Name} took {completed.Duration}"));

            registerUser.Canceled.Subscribe(canceled =>
                Console.WriteLine($"RegisterUser operation for {canceled.Context.Name} was canceled"));

            using (var timer = registerUser.StartNewTimer(new RegisterUser { Name = "Bob"}))
            // "Started RegisterUser operation for Bob"
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            // "RegisterUser operation for Bob took 00:00:01.0006389"

            using (var timer = registerUser.StartNewTimer(new RegisterUser { Name = "Alice" }))
            // "Started RegisterUser operation for Alice"
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                timer.Stop();
                // "RegisterUser operation for Alice took 00:00:01.0003205"
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            using (var timer = registerUser.StartNewTimer(new RegisterUser { Name = "John" }))
            // "Started RegisterUser operation for John"
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                timer.Cancel();
                // "RegisterUser operation for John was canceled"
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }
    }
}
