using Serilog;
using Serilog.Events;
using System;
using System.Linq;
using System.Reactive.Linq;
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
            
            dataStore.ReadRandomBytesOperation.Completed
                .Subscribe(new CompletedOperationLogger(
                    dataStoreLogger.ForContext("OperationName", "ReadRandomBytes"),
                    LogEventLevel.Information,
                    "{OperationName} operation {OperationId} completed in {DurationSeconds}s"));
            
            dataStore.ReadRandomByteOperation.Completed
                .Buffer(TimeSpan.FromSeconds(10))
                .TimeInterval()
                .Subscribe(new CompletedOperationsLogger(
                    dataStoreLogger.ForContext("OperationName", "ReadRandomByte"),
                    LogEventLevel.Information,
                    "Average time taken to complete {OperationName} operations was {AverageDurationSeconds}s over the last {SamplePeriodDurationSeconds}s"));

            dataStore.ReadRandomByteOperation.Completed
                .Buffer(dataStore.ReadRandomBytesOperation)
                .TimeInterval()
                .Subscribe(new CompletedOperationsLogger(
                    dataStoreLogger.ForContext("OperationName", "ReadRandomByte"),
                    LogEventLevel.Information,
                    "Average time taken to complete {OperationName} operations was {AverageDurationSeconds}s over the last {SamplePeriodDurationSeconds}s"));

            dataStore.RandomGauge.Sampled
                .Subscribe(sampled =>
                    dataStoreLogger
                        .ForContext("GaugeName", "RandomGauge")
                        .ForContext("Value", sampled.Value)
                        .Information("{GaugeName} value is {Value}"));

            dataStore.BytesReadCounter.Incremented
                .Buffer(TimeSpan.FromSeconds(8))
                .TimeInterval()
                .Subscribe(buffer =>
                    dataStoreLogger
                        .ForContext("SamplePeriodDurationSeconds", buffer.Interval.TotalSeconds)
                        .ForContext("TotalCount", buffer.Value.Sum(incremented => incremented.Increment))
                        .Information("Total number of bytes read over the last {SamplePeriodDurationSeconds}s was {TotalCount}"));
            
            while (true)
            {
                var buffer = await dataStore.ReadRandomBytesAsync(4).ConfigureAwait(false);
                Log.Information("Read random bytes from data store: {Base64Bytes}", Convert.ToBase64String(buffer));
            }
        }
    }
}
