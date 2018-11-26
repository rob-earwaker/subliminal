using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lognostics.Serilog.TestApp
{
    internal class DataStore
    {
        private readonly Random _random;

        public Operation ReadRandomBytesOperation { get; }
        public Operation ReadRandomByteOperation { get; }
        public Metric<int> RandomMetric { get; }
        public Counter BytesReadCounter { get; }

        public DataStore()
        {
            _random = new Random();
            ReadRandomBytesOperation = new Operation();
            ReadRandomByteOperation = new Operation();
            RandomMetric = new Metric<int>();
            BytesReadCounter = new Counter();
        }

        public async Task<byte[]> ReadRandomBytesAsync(int bufferSize)
        {
            using (var operationScope = ReadRandomBytesOperation.StartNew())
            {
                operationScope.AddContext("BufferSize", bufferSize);
                var buffer = await Task.WhenAll(new object[bufferSize].Select(_ => ReadRandomByteAsync()));
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
                RandomMetric.LogValue(_random.Next());
                return buffer[0];
            }
        }
    }
}
