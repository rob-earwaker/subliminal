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
                        "Total processor usage over the last {Interval} was {Percentage}%",
                        processorUsage.Interval, processorUsage.TotalPercentage));

            var threadPoolMonitor = ThreadPoolMonitor.ForManagedThreadPool(TimeSpan.FromSeconds(5));

            threadPoolMonitor.ActiveWorkerThreads
                .Subscribe(activeWorkerThreads =>
                    Log.Information("Active worker thread count is {ActiveWorkerThreads}", activeWorkerThreads));

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

            dataStore.RandomGauge
                .Buffer(100)
                .Select(samples => samples.Average())
                .TimeInterval()
                .Subscribe(averageValue =>
                    dataStoreLogger
                        .ForContext("MetricName", "RandomMetric")
                        .ForContext("AverageValue", averageValue.Value)
                        .ForContext("SampleInterval", averageValue.Interval)
                        .Information("Average {MetricName} value was {AverageValue} over the last {SampleInterval}"));

            dataStore.BytesReadCounter.Rate()
                .Buffer(TimeSpan.FromSeconds(5))
                .Select(bitRates => bitRates.Average())
                .Subscribe(averageBitRate =>
                    dataStoreLogger
                        .ForContext("ByteRate", averageBitRate.BytesPerSecond)
                        .ForContext("Interval", averageBitRate.Interval)
                        .Information("Read speed was {ByteRate}B/s over the last {Interval}"));

            while (true)
            {
                var buffer = await dataStore.ReadRandomBytesAsync(128).ConfigureAwait(false);
                Log.Information("Read random bytes from data store: {Base64Bytes}", Convert.ToBase64String(buffer));
            }
        }
    }
}
