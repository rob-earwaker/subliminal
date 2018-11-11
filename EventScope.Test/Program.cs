using EventScope.Logging.Serilog;
using Serilog;
using Serilog.Events;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventScope.Test
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();

            var dataStore = new DataStore();

            var readRandomBytesSubscription = new EventSubscription<OperationCompletedEventArgs>(
                dataStore.ReadRandomBytesOperation,
                new OperationDurationLogger("ReadRandomBytes", Log.Logger.ForContext(dataStore.GetType())));

            var readRandomByteSubscription = new EventSubscription<OperationCompletedEventArgs>(
                dataStore.ReadRandomByteOperation,
                new OperationDurationSummaryLogger("ReadRandomByte", Log.Logger.ForContext(dataStore.GetType()), LogEventLevel.Debug));

            var subscriptionLifetimeScopeSource = new SubscriptionLifetimeScopeSource();
            subscriptionLifetimeScopeSource.HandleEvent(new object(), new ScopeStartedEventArgs(Scope.RootScope));
            subscriptionLifetimeScopeSource.AddHandler(readRandomBytesSubscription);
            subscriptionLifetimeScopeSource.AddHandler(dataStore.ReadRandomBytesOperation);
            subscriptionLifetimeScopeSource.AddHandler(dataStore.ReadRandomByteOperation);

            dataStore.ReadRandomBytesOperation.AddHandler(readRandomByteSubscription);

            for (var index = 0; index < 8; index++)
            {
                var buffer = await dataStore.ReadRandomBytesAsync(4).ConfigureAwait(false);
                Log.Information("Read random bytes from data store: {Base64Bytes}", Convert.ToBase64String(buffer));
            }

            Log.Information("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
