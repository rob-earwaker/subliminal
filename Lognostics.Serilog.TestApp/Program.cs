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
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            var dataStore = new DataStore();

            var dataStoreLogger = Log.Logger.ForContext(dataStore.GetType());

            var readRandomBytesLogger = new OperationDurationLogger("ReadRandomBytes", dataStoreLogger);
            
            var readRandomByteLogger = ScopedEventHandler.Create(
                EventAggregator.Create(new OperationDurationSummaryLogger("ReadRandomByte", dataStoreLogger)),
                new AggregateScopeSource(
                    PeriodicScopeSource.StartNew(TimeSpan.FromSeconds(10)),
                    dataStore.ReadRandomBytesOperation));

            var randomMetricLogger = new MetricValueLogger<int>("RandomMetric", "{MetricName} value is {Value}", dataStoreLogger);

            dataStore.ReadRandomBytesOperation.Completed += readRandomBytesLogger.HandleEvent;
            dataStore.ReadRandomByteOperation.Completed += readRandomByteLogger.HandleEvent;
            dataStore.RandomMetric.Sampled += randomMetricLogger.HandleEvent;

            while (true)
            {
                var buffer = await dataStore.ReadRandomBytesAsync(4).ConfigureAwait(false);
                Log.Information("Read random bytes from data store: {Base64Bytes}", Convert.ToBase64String(buffer));
            }
        }
    }
}
