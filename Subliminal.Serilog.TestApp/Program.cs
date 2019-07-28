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

            currentProcessMonitor.CpuUsage.Values
                .Subscribe(processorUsage =>
                    Log.Information(
                        "CPU usage over the last {Interval} was {Percentage}%",
                        processorUsage.Interval, processorUsage.Percentage));

            var threadPoolMonitor = ThreadPoolMonitor.ForManagedThreadPool(TimeSpan.FromSeconds(5));

            threadPoolMonitor.ActiveWorkerThreads.Values
                .Subscribe(activeWorkerThreads =>
                    Log.Information("Active worker thread count is {ActiveWorkerThreads}", activeWorkerThreads));

            var dataStore = new DataStore();

            var dataStoreLogger = Log.Logger.ForContext(dataStore.GetType());

            dataStore.ReadRandomBytesOperation.Completed.Events
                .Subscribe(new CompletedOperationLogger(
                    dataStoreLogger.ForContext("OperationName", "ReadRandomBytes"),
                    LogEventLevel.Information,
                    "{OperationName} operation {ExecutionId} completed in {DurationSeconds}s"));

            dataStore.ReadRandomByteOperation.Completed.Events
                .Buffer(TimeSpan.FromSeconds(10))
                .TimeInterval()
                .Subscribe(new CompletedOperationsLogger(
                    dataStoreLogger.ForContext("OperationName", "ReadRandomByte"),
                    LogEventLevel.Information,
                    "Average time taken to complete {OperationName} operations was {AverageDurationSeconds}s over the last {SamplePeriodDurationSeconds}s"));

            dataStore.ReadRandomByteOperation.Completed.Events
                .Buffer(dataStore.ReadRandomBytesOperation)
                .TimeInterval()
                .Subscribe(new CompletedOperationsLogger(
                    dataStoreLogger.ForContext("OperationName", "ReadRandomByte"),
                    LogEventLevel.Information,
                    "Average time taken to complete {OperationName} operations was {AverageDurationSeconds}s over the last {SamplePeriodDurationSeconds}s"));

            dataStore.RandomMetric.Values
                .Buffer(100)
                .Select(values => values.Average())
                .TimeInterval()
                .Subscribe(averageValue =>
                    dataStoreLogger
                        .ForContext("MetricName", "RandomMetric")
                        .ForContext("AverageValue", averageValue.Value)
                        .ForContext("SampleInterval", averageValue.Interval)
                        .Information("Average {MetricName} value was {AverageValue} over the last {SampleInterval}"));

            dataStore.BytesReadCounter.RateOfChange
                .Buffer(TimeSpan.FromSeconds(5))
                .Select(ratesOfChange => ratesOfChange.Average())
                .Subscribe(rateOfChange =>
                    dataStoreLogger
                        .ForContext("ByteRate", rateOfChange.DeltaPerSecond)
                        .ForContext("RateInterval", rateOfChange.Interval)
                        .Information("Read speed was {ByteRate}B/s over the last {RateInterval}"));

            while (true)
            {
                var buffer = await dataStore.ReadRandomBytesAsync(1024).ConfigureAwait(false);
                Log.Information("Read random bytes from data store: {Base64Bytes}", Convert.ToBase64String(buffer));
            }
        }
    }
}
