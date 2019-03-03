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

            var currentProcessMonitor = ProcessMonitor.ForCurrentProcess(TimeSpan.FromSeconds(5));

            currentProcessMonitor.ProcessorUsage
                .Subscribe(processorUsage =>
                    Log.Information(
                        "CPU usage over the last {Interval} was {Percentage}%",
                        processorUsage.Interval, processorUsage.Percentage));

            var dataStore = new DataStore();

            var dataStoreLogger = Log.Logger.ForContext(dataStore.GetType());

            dataStore.ReadRandomBytesOperation.OperationCompleted
                .Subscribe(new CompletedOperationLogger(
                    dataStoreLogger.ForContext("OperationName", "ReadRandomBytes"),
                    LogEventLevel.Information,
                    "{OperationName} operation {OperationId} completed in {DurationSeconds}s"));

            dataStore.ReadRandomByteOperation.OperationCompleted
                .Buffer(TimeSpan.FromSeconds(10))
                .TimeInterval()
                .Subscribe(new CompletedOperationsLogger(
                    dataStoreLogger.ForContext("OperationName", "ReadRandomByte"),
                    LogEventLevel.Information,
                    "Average time taken to complete {OperationName} operations was {AverageDurationSeconds}s over the last {SamplePeriodDurationSeconds}s"));

            dataStore.ReadRandomByteOperation.OperationCompleted
                .Buffer(dataStore.ReadRandomBytesOperation)
                .TimeInterval()
                .Subscribe(new CompletedOperationsLogger(
                    dataStoreLogger.ForContext("OperationName", "ReadRandomByte"),
                    LogEventLevel.Information,
                    "Average time taken to complete {OperationName} operations was {AverageDurationSeconds}s over the last {SamplePeriodDurationSeconds}s"));

            dataStore.RandomMetric
                .Subscribe(randomValue =>
                    dataStoreLogger
                        .ForContext("MetricName", "RandomMetric")
                        .ForContext("Value", randomValue)
                        .Information("{MetricName} value is {Value}"));

            dataStore.BytesReadCounter
                .Buffer(TimeSpan.FromSeconds(8))
                .TimeInterval()
                .Subscribe(buffer =>
                    dataStoreLogger
                        .ForContext("SamplePeriodDurationSeconds", buffer.Interval.TotalSeconds)
                        .ForContext("TotalCount", buffer.Value.Sum())
                        .Information("Total number of bytes read over the last {SamplePeriodDurationSeconds}s was {TotalCount}"));

            while (true)
            {
                var buffer = await dataStore.ReadRandomBytesAsync(4).ConfigureAwait(false);
                Log.Information("Read random bytes from data store: {Base64Bytes}", Convert.ToBase64String(buffer));
            }
        }
    }
}
