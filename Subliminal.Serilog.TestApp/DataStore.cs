using System;
using System.Linq;
using System.Threading.Tasks;

namespace Subliminal.Serilog.TestApp
{
    internal class DataStore
    {
        private readonly Random _random;
        private readonly Metric<int> _randomMetric;
        private readonly Counter _bytesReadCounter;

        public DataStore()
        {
            _random = new Random();
            _randomMetric = new Metric<int>();
            _bytesReadCounter = new Counter();

            ReadRandomBytesOperation = new Operation();
            ReadRandomByteOperation = new Operation();
        }

        public IMetric<int> RandomMetric => _randomMetric;
        public ICounter BytesReadCounter => _bytesReadCounter;

        public Operation ReadRandomBytesOperation { get; }
        public Operation ReadRandomByteOperation { get; }

        public async Task<byte[]> ReadRandomBytesAsync(int bufferSize)
        {
            using (var operationScope = ReadRandomBytesOperation.StartNew())
            {
                var readRandomByteTasks = new object[bufferSize].Select(_ => ReadRandomByteAsync()).ToArray();
                var buffer = await Task.WhenAll(readRandomByteTasks).ConfigureAwait(false);
                _bytesReadCounter.IncrementBy(bufferSize);
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
                _randomMetric.RecordValue(_random.Next());
                return buffer[0];
            }
        }
    }
}
