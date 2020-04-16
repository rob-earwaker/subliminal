using System;
using System.Linq;
using System.Threading.Tasks;

namespace Subliminal.Sample.Serilog
{
    internal class DataStore
    {
        private static readonly Random _random = new Random();
        private static readonly object _randomLockObject = new object();

        public static readonly Gauge<int> RandomGauge = new Gauge<int>();
        public static readonly Counter BytesReadCounter = new Counter();
        public static readonly Operation ReadRandomBytesOperation = new Operation();
        public static readonly Operation ReadRandomByteOperation = new Operation();

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
                int randomValue;

                lock (_randomLockObject)
                {
                    _random.NextBytes(buffer);
                    randomDelay = _random.NextDouble();
                    randomValue = _random.Next();
                }

                await Task.Delay(TimeSpan.FromSeconds(randomDelay)).ConfigureAwait(false);

                RandomGauge.LogValue(randomValue);
                BytesReadCounter.IncrementBy(buffer.Length);

                return buffer[0];
            }
        }
    }
}
