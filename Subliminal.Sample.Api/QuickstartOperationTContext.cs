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
                Console.WriteLine($"Started registration of user '{started.Context.Name}'"));

            registerUser.Completed.Subscribe(completed =>
                Console.WriteLine(
                    $"Successfully registered user '{completed.Context.Name}' in {completed.Duration}"));

            registerUser.Canceled.Subscribe(canceled =>
                Console.WriteLine(
                    $"Canceled registration of user '{canceled.Context.Name}' after {canceled.Duration}"));

            registerUser.Ended.Subscribe(ended =>
                Console.WriteLine(
                    $"Finished registration of user '{ended.Context.Name}' after {ended.Duration}"));
            
            using (var timer = registerUser.StartNewTimer(new RegisterUser { Name = "bob"}))
            // "Started registration of user 'bob'"
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            // "Successfully registered user 'bob' in 00:00:01.0004845"
            // "Finished registration of user 'bob' after 00:00:01.0004845"

            using (var timer = registerUser.StartNewTimer(new RegisterUser { Name = "alice" }))
            // "Started registration of user 'alice'"
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                timer.Complete();
                // "Successfully registered user 'alice' in 00:00:01.0005217"
                // "Finished registration of user 'alice' after 00:00:01.0005217"
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            using (var timer = registerUser.StartNewTimer(new RegisterUser { Name = "john" }))
            // "Started registration of user 'john'"
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                timer.Cancel();
                // "Canceled registration of user 'john' after 00:00:01.0008496"
                // "Finished registration of user 'john' after 00:00:01.0008496"
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }
    }
}
