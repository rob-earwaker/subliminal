using Serilog;
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
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            var currentProcessMonitor = ProcessMonitor.ForCurrentProcess(TimeSpan.FromSeconds(5));

            currentProcessMonitor.ProcessorUsage
                .Subscribe(processorUsage =>
                    Log.Information(
                        "Total processor usage over the last {Interval} was {Percentage}%",
                        processorUsage.Interval, processorUsage.TotalPercentage));

            currentProcessMonitor.WorkingSet
                .RateOfChange()
                .Subscribe(rate =>
                    Log.Information("Working set usage changed by {Delta}MB in the last {Interval}",
                        rate.Delta.Megabytes, rate.Interval));

            var threadPoolMonitor = ThreadPoolMonitor.ForManagedThreadPool(TimeSpan.FromSeconds(5));

            threadPoolMonitor.ActiveWorkerThreads
                .Subscribe(activeWorkerThreads =>
                    Log.Information("Active worker thread count is {ActiveWorkerThreads}", activeWorkerThreads));

            var dataStoreLogger = Log.Logger.ForContext<DataStore>();

            DataStore.ReadRandomBytesOperation.Completed
                .Subscribe(operation =>
                    dataStoreLogger.Information("{OperationName} operation {OperationId} completed in {Duration}",
                        "ReadRandomBytes", operation.OperationId, operation.Duration));

            DataStore.ReadRandomByteOperation.Completed
                .Buffer(TimeSpan.FromSeconds(10))
                .Where(buffer => buffer.Any())
                .TimeInterval()
                .Subscribe(operations =>
                    dataStoreLogger.Information(
                        "Average {OperationName} duration was {AverageDuration} over the last {SamplePeriodDuration}",
                        "ReadRandomByte", operations.Value.Average(operation => operation.Duration), operations.Interval));

            DataStore.ReadRandomByteOperation.Completed
                .Buffer(DataStore.ReadRandomBytesOperation, operation => operation.Ended)
                .TimeInterval()
                .Subscribe(operations =>
                    dataStoreLogger.Information(
                        "Average {OperationName} duration was {AverageDuration} over the last {SamplePeriodDuration}",
                        "ReadRandomByte", operations.Value.Average(operation => operation.Duration), operations.Interval));

            DataStore.RandomGauge
                .Buffer(128)
                .Select(samples => samples.Average())
                .TimeInterval()
                .Subscribe(averageValue =>
                    dataStoreLogger.Information("Average {MetricName} value was {AverageValue} over the last {Interval}",
                        "RandomMetric", averageValue.Value, averageValue.Interval));

            DataStore.RandomGauge
                .RateOfChange()
                .Buffer(128)
                .Select(rates => rates.Average())
                .Subscribe(rate =>
                    dataStoreLogger.Information("{MetricName} rate was {Rate}/s over the last {Interval}",
                        "RandomMetric", rate.Delta / rate.Interval.TotalSeconds, rate.Interval));

            DataStore.BytesReadCounter
                .IncrementRate()
                .Buffer(TimeSpan.FromSeconds(5))
                .Select(bitRates => bitRates.Average())
                .Subscribe(byteRate =>
                    dataStoreLogger.Information("Read speed was {BytesPerSecond}B/s over the last {Interval}",
                        byteRate.Delta.Bytes / byteRate.Interval.TotalSeconds, byteRate.Interval));

            var dataStore = new DataStore();

            while (true)
            {
                var buffer = await dataStore.ReadRandomBytesAsync(128).ConfigureAwait(false);
                Log.Information("Read random bytes from data store: {Base64Bytes}", Convert.ToBase64String(buffer));
            }
        }
    }
}
