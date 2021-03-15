using Serilog;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Subliminal.Sample.Serilog
{
    internal static class Program
    {
        public static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            global::Serilog.Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            var currentProcessMonitor = ProcessMonitor.ForCurrentProcess(TimeSpan.FromSeconds(5));

            //currentProcessMonitor.ProcessorUsage
            //    .Subscribe(processorUsage =>
            //        Log.Information(
            //            "Total processor usage over the last {Interval} was {Percentage}%",
            //            processorUsage.Interval, processorUsage.TotalPercentage));

            currentProcessMonitor.WorkingSet
                .RateOfChange()
                .Subscribe(rate =>
                    Log.Information("Working set usage changed by {Delta}MB in the last {Interval}",
                        rate.Delta / 1E6, rate.Interval));

            var threadPoolMonitor = ThreadPoolMonitor.ForManagedThreadPool(TimeSpan.FromSeconds(5));

            threadPoolMonitor.ActiveWorkerThreads
                .Subscribe(activeWorkerThreads =>
                    Log.Information("Active worker thread count is {ActiveWorkerThreads}", activeWorkerThreads));

            var dataStoreLogger = Log.Logger.ForContext<DataStore>();

            DataStore.ReadRandomBytesOperation.Completed
                .Subscribe(completed =>
                    dataStoreLogger.Information("{OperationName} operation {OperationId} completed after {Duration}",
                        "ReadRandomBytes", completed.OperationId, completed.Duration));

            DataStore.ReadRandomByteOperation.Completed
                .Buffer(TimeSpan.FromSeconds(10))
                .Where(buffer => buffer.Any())
                .TimeInterval()
                .Subscribe(completedOperations =>
                    dataStoreLogger.Information(
                        "Average {OperationName} duration was {AverageDuration} over the last {SamplePeriodDuration}",
                        "ReadRandomByte", completedOperations.Value.Average(completed => completed.Duration),
                        completedOperations.Interval));
            
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
                .Rate()
                .Buffer(TimeSpan.FromSeconds(5))
                .Select(bitRates => bitRates.Average())
                .Subscribe(byteRate =>
                    dataStoreLogger.Information("Read speed was {BytesPerSecond}B/s over the last {Interval}",
                        byteRate.Delta / byteRate.Interval.TotalSeconds, byteRate.Interval));

            var dataStore = new DataStore();

            while (true)
            {
                var buffer = await dataStore.ReadRandomBytesAsync(128).ConfigureAwait(false);
                Log.Information("Read random bytes from data store: {Base64Bytes}", Convert.ToBase64String(buffer));
            }
        }
    }
}
