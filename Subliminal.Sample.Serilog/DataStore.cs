using System;
using System.Linq;
using System.Threading.Tasks;

namespace Subliminal.Sample.Serilog
{
    internal class DataStore
    {
        private static readonly Random _random = new Random();
        private static readonly object _randomLockObject = new object();

        public static readonly Gauge RandomGauge = new Gauge();
        public static readonly Counter BytesReadCounter = new Counter();
        public static readonly OperationLog ReadRandomBytesOperation = new OperationLog();
        public static readonly OperationLog ReadRandomByteOperation = new OperationLog();

        public async Task<byte[]> ReadRandomBytesAsync(int bufferSize)
        {
            using (ReadRandomBytesOperation.StartNewTimer())
            {
                var readRandomByteTasks = new object[bufferSize].Select(_ => ReadRandomByteAsync()).ToArray();
                var buffer = await Task.WhenAll(readRandomByteTasks).ConfigureAwait(false);
                return buffer;
            }
        }

        private async Task<byte> ReadRandomByteAsync()
        {
            using (ReadRandomByteOperation.StartNewTimer())
            {
                var buffer = new byte[1];
                double randomDelay;
                double randomValue;

                lock (_randomLockObject)
                {
                    _random.NextBytes(buffer);
                    randomDelay = _random.NextDouble();
                    randomValue = _random.NextDouble();
                }

                await Task.Delay(TimeSpan.FromSeconds(randomDelay)).ConfigureAwait(false);

                RandomGauge.LogValue(randomValue);
                BytesReadCounter.IncrementBy(buffer.Length);

                return buffer[0];
            }
        }
    }
}
