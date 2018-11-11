using Serilog;
using System;
using System.Threading.Tasks;

namespace Lognostics.Serilog.TestApp
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

            var dataStoreLogger = Log.Logger.ForContext(dataStore.GetType());

            var readRandomBytesLogger = new OperationDurationLogger("ReadRandomBytes", dataStoreLogger);
            var readRandomByteLogger = new OperationDurationLogger("ReadRandomByte", dataStoreLogger);

            var readRandomByteSummaryLogger = ScopedEventHandler.Create(
                AggregateEventHandler.Create(new OperationDurationSummaryLogger("ReadRandomByte", dataStoreLogger)),
                dataStore.ReadRandomBytesOperation);

            dataStore.ReadRandomBytesOperation.Completed += readRandomBytesLogger.HandleEvent;
            dataStore.ReadRandomByteOperation.Completed += readRandomByteLogger.HandleEvent;
            dataStore.ReadRandomByteOperation.Completed += readRandomByteSummaryLogger.HandleEvent;

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
