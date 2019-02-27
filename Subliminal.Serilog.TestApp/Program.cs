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

            var processMonitor = new ProcessMonitor(TimeSpan.FromSeconds(5));

            processMonitor.CpuUsageSource.Samples
                .Subscribe(sample => Log.Information(
                    "[{Timestamp}] [{Interval}] CPU usage: {CpuUsage}%",
                    sample.Timestamp.ToString("o"), sample.Interval, sample.Value * 100, sample.Interval));

            processMonitor.TotalProcessorTimeSource.Samples
                .Subscribe(sample => Log.Information(
                    "[{Timestamp}] [{Interval}] Total CPU time: {TotalCpuTime}s",
                    sample.Timestamp.ToString("o"), sample.Interval, sample.Value));

            processMonitor.WorkingSet64Source.Samples
                .Subscribe(sample => Log.Information(
                    "[{Timestamp}] [{Interval}] RAM working set: {WorkingSet}MB",
                    sample.Timestamp.ToString("o"), sample.Interval, sample.Value / 1E6));

            processMonitor.PrivateMemorySize64Source.Samples
                .Subscribe(sample => Log.Information(
                    "[{Timestamp}] [{Interval}] RAM private: {WorkingSet}MB",
                    sample.Timestamp.ToString("o"), sample.Interval, sample.Value / 1E6));

            processMonitor.VirtualMemorySize64Source.Samples
                .Subscribe(sample => Log.Information(
                    "[{Timestamp}] [{Interval}] RAM virtual: {WorkingSet}MB",
                    sample.Timestamp.ToString("o"), sample.Interval, sample.Value / 1E6));

            //dataStore.ReadRandomBytesOperation.Completed
            //    .Subscribe(new CompletedOperationLogger(
            //        dataStoreLogger.ForContext("OperationName", "ReadRandomBytes"),
            //        LogEventLevel.Information,
            //        "{OperationName} operation {OperationId} completed in {DurationSeconds}s"));

            //dataStore.ReadRandomByteOperation.Completed
            //    .Buffer(TimeSpan.FromSeconds(10))
            //    .TimeInterval()
            //    .Subscribe(new CompletedOperationsLogger(
            //        dataStoreLogger.ForContext("OperationName", "ReadRandomByte"),
            //        LogEventLevel.Information,
            //        "Average time taken to complete {OperationName} operations was {AverageDurationSeconds}s over the last {SamplePeriodDurationSeconds}s"));

            //dataStore.ReadRandomByteOperation.Completed
            //    .Buffer(dataStore.ReadRandomBytesOperation)
            //    .TimeInterval()
            //    .Subscribe(new CompletedOperationsLogger(
            //        dataStoreLogger.ForContext("OperationName", "ReadRandomByte"),
            //        LogEventLevel.Information,
            //        "Average time taken to complete {OperationName} operations was {AverageDurationSeconds}s over the last {SamplePeriodDurationSeconds}s"));

            //dataStore.RandomGauge.Sampled
            //    .Subscribe(sample =>
            //        dataStoreLogger
            //            .ForContext("GaugeName", "RandomGauge")
            //            .ForContext("Value", sample.Value)
            //            .Information("{GaugeName} value is {Value}"));

            //dataStore.BytesReadCounter.Incremented
            //    .Buffer(TimeSpan.FromSeconds(8))
            //    .TimeInterval()
            //    .Subscribe(buffer =>
            //        dataStoreLogger
            //            .ForContext("SamplePeriodDurationSeconds", buffer.Interval.TotalSeconds)
            //            .ForContext("TotalCount", buffer.Value.Sum(incremented => incremented.Increment))
            //            .Information("Total number of bytes read over the last {SamplePeriodDurationSeconds}s was {TotalCount}"));

            while (true)
            {
                for (int i = 0; i < 1E8; i++)
                {
                    Log.Verbose("{Value}", Math.Pow(i, 2));
                }

                var buffer = await dataStore.ReadRandomBytesAsync(4).ConfigureAwait(false);
                Log.Information("Read random bytes from data store: {Base64Bytes}", Convert.ToBase64String(buffer));
            }
        }
    }
}
