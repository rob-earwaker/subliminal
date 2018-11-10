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

            var readRandomBytesSubscription = new Subscription<OperationCompletedEventArgs>(
                dataStore.ReadRandomBytesOperation,
                new OperationDurationLogger("ReadRandomBytes", Log.Logger.ForContext(dataStore.GetType())));

            var subscriptionLifetimeScopeSource = new SubscriptionLifetimeScopeSource();
            subscriptionLifetimeScopeSource.AddHandler(readRandomBytesSubscription);

            var readRandomByteSubscription = new Subscription<OperationCompletedEventArgs>(
                dataStore.ReadRandomByteOperation,
                new OperationDurationLogger("ReadRandomByte", Log.Logger.ForContext(dataStore.GetType()), LogEventLevel.Debug));

            dataStore.ReadRandomBytesOperation.AddHandler(readRandomByteSubscription);

            for (var index = 0; index < 8; index++)
            {
                var buffer = await dataStore.ReadRandomBytesAsync(4).ConfigureAwait(false);
                var hexString = string.Concat(buffer.Select(b => $"{b:X2}"));
                Log.Information("Read random bytes from data store: {BytesHex}", hexString);
            }

            Log.Information("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
