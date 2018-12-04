using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace Subliminal.Serilog.TestApp
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
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            var dataStore = new DataStore();

            var dataStoreLogger = Log.Logger.ForContext(dataStore.GetType());

            var readRandomBytesLogger = new OperationDurationLogger(
                "Took {OperationDurationSeconds}s to {OperationName}",
                dataStoreLogger.ForContext("OperationName", "ReadRandomBytes"),
                LogEventLevel.Information);

            var readRandomByteLogger = ScopedEventHandler.Create(
                EventAggregator.Create(
                    new OperationDurationSummaryLogger(
                        "Average time taken to {OperationName} was {AverageDurationSeconds}s over the last {SamplePeriodDurationSeconds}s",
                        dataStoreLogger.ForContext("OperationName", "ReadRandomByte"),
                        LogEventLevel.Information)),
                new AggregateScopeSource(
                    PeriodicScopeSource.StartNew(TimeSpan.FromSeconds(10)),
                    dataStore.ReadRandomBytesOperation));

            var randomGaugeLogger = new GaugeValueLogger<int>(
                "{GaugeName} value is {Value}",
                dataStoreLogger.ForContext("GaugeName", "RandomGauge"),
                LogEventLevel.Information);

            var readByteCountLogger = ScopedEventHandler.Create(
                EventAggregator.Create(
                    new TotalCountLogger(
                        "Total number of bytes read over the last {SamplePeriodDurationSeconds}s was {TotalCount}",
                        dataStoreLogger,
                        LogEventLevel.Information)),
                PeriodicScopeSource.StartNew(TimeSpan.FromSeconds(8)));

            dataStore.ReadRandomBytesOperation.Completed += readRandomBytesLogger.HandleEvent;
            dataStore.ReadRandomByteOperation.Completed += readRandomByteLogger.HandleEvent;
            dataStore.RandomGauge.Sampled.Subscribe(randomGaugeLogger);
            dataStore.BytesReadCounter.Incremented += readByteCountLogger.HandleEvent;

            while (true)
            {
                var buffer = await dataStore.ReadRandomBytesAsync(4).ConfigureAwait(false);
                Log.Information("Read random bytes from data store: {Base64Bytes}", Convert.ToBase64String(buffer));
            }
        }
    }
}
