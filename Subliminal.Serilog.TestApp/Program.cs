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
                .Subscribe(operationScope =>
                    dataStoreLogger
                        .ForContext("OperationName", "ReadRandomBytes")
                        .ForContext("OperationDurationSeconds", operationScope.Duration.TotalSeconds)
                        .Information("Took {OperationDurationSeconds}s to {OperationName}"));
            
            dataStore.ReadRandomByteOperation.Completed
                .Buffer(TimeSpan.FromSeconds(10))
                .TimeInterval()
                .Subscribe(buffer =>
                    dataStoreLogger
                        .ForContext("AverageDurationSeconds", buffer.Value.Average(operationScope => operationScope.Duration.TotalSeconds))
                        .ForContext("SamplePeriodDurationSeconds", buffer.Interval.TotalSeconds)
                        .ForContext("OperationName", "ReadRandomByte")
                        .Information("Average time taken to {OperationName} was {AverageDurationSeconds}s over the last {SamplePeriodDurationSeconds}s"));

            dataStore.ReadRandomByteOperation.Completed
                .Buffer(dataStore.ReadRandomBytesOperation.Started, readRandomBytesOperation => readRandomBytesOperation.Ended)
                .TimeInterval()
                .Subscribe(buffer =>
                    dataStoreLogger
                        .ForContext("AverageDurationSeconds", buffer.Value.Average(operationScope => operationScope.Duration.TotalSeconds))
                        .ForContext("SamplePeriodDurationSeconds", buffer.Interval.TotalSeconds)
                        .ForContext("OperationName", "ReadRandomByte")
                        .Information("Average time taken to {OperationName} was {AverageDurationSeconds}s over the last {SamplePeriodDurationSeconds}s"));
            
            dataStore.RandomGauge.Sampled
                .Subscribe(value =>
                    dataStoreLogger
                        .ForContext("GaugeName", "RandomGauge")
                        .ForContext("Value", value)
                        .Information("{GaugeName} value is {Value}"));

            dataStore.BytesReadCounter.Incremented
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
