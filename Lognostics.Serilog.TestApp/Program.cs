using Serilog;
using System;
using System.Threading;
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
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            var dataStore = new DataStore();

            var dataStoreLogger = Log.Logger.ForContext(dataStore.GetType());

            var readRandomBytesLogger = new OperationDurationLogger("ReadRandomBytes", dataStoreLogger);
            var readRandomByteLogger = new OperationDurationSummaryLogger("ReadRandomByte", dataStoreLogger);

            var readRandomByteSummaryLogger = ScopedEventHandler.Create(
                AggregateEventHandler.Create(readRandomByteLogger),
                dataStore.ReadRandomBytesOperation);
            
            var periodicScopeSource = new PeriodicScopeSource(TimeSpan.FromSeconds(10));
            var cancellationTokenSource = new CancellationTokenSource();
            var task = periodicScopeSource.StartAsync(cancellationTokenSource.Token);

            var readRandomBytePeriodicSummaryLogger = ScopedEventHandler.Create(
                AggregateEventHandler.Create(readRandomByteLogger),
                periodicScopeSource);

            dataStore.ReadRandomBytesOperation.Completed += readRandomBytesLogger.HandleEvent;
            dataStore.ReadRandomByteOperation.Completed += readRandomByteSummaryLogger.HandleEvent;
            dataStore.ReadRandomByteOperation.Completed += readRandomBytePeriodicSummaryLogger.HandleEvent;

            while (true)
            {
                var buffer = await dataStore.ReadRandomBytesAsync(4).ConfigureAwait(false);
                Log.Information("Read random bytes from data store: {Base64Bytes}", Convert.ToBase64String(buffer));
            }

            //Log.Information("Press any key to exit...");
            //Console.ReadKey();
        }
    }
}
