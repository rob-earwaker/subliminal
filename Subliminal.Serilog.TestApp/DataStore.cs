using System;
using System.Linq;
using System.Threading.Tasks;

namespace Subliminal.Serilog.TestApp
{
    internal class DataStore
    {
        private readonly Random _random;

        public Operation ReadRandomBytesOperation { get; }
        public Operation ReadRandomByteOperation { get; }
        public Gauge<int> RandomGauge { get; }
        public Counter BytesReadCounter { get; }

        public DataStore()
        {
            _random = new Random();
            ReadRandomBytesOperation = new Operation();
            ReadRandomByteOperation = new Operation();
            RandomGauge = new Gauge<int>();
            BytesReadCounter = new Counter();
        }

        public async Task<byte[]> ReadRandomBytesAsync(int bufferSize)
        {
            using (var operationScope = ReadRandomBytesOperation.StartNew())
            {
                operationScope.AddContext("BufferSize", bufferSize);
                var readRandomByteTasks = new object[bufferSize].Select(_ => ReadRandomByteAsync()).ToArray();
                var buffer = await Task.WhenAll(readRandomByteTasks).ConfigureAwait(false);
                BytesReadCounter.IncrementBy(bufferSize);
                return buffer;
            }
        }

        private async Task<byte> ReadRandomByteAsync()
        {
            using (ReadRandomByteOperation.StartNew())
            {
                var buffer = new byte[1];
                _random.NextBytes(buffer);
                await Task.Delay(TimeSpan.FromSeconds(_random.NextDouble())).ConfigureAwait(false);
                RandomGauge.LogValue(_random.Next());
                return buffer[0];
            }
        }
    }
}
