using System;
using System.Linq;
using System.Threading.Tasks;

namespace Subliminal.Serilog.TestApp
{
    internal class DataStore
    {
        private readonly Random _random;
        private readonly Gauge<int> _randomMetric;
        private readonly Counter _bytesReadCounter;
        private readonly Operation _readRandomBytesOperation;
        private readonly Operation _readRandomByteOperation;

        public DataStore()
        {
            _random = new Random();
            _randomMetric = new Gauge<int>();
            _bytesReadCounter = new Counter();
            _readRandomBytesOperation = new Operation();
            _readRandomByteOperation = new Operation();
        }

        public IGauge<int> RandomMetric => _randomMetric;
        public ICounter BytesReadCounter => _bytesReadCounter;
        public IOperation ReadRandomBytesOperation => _readRandomBytesOperation;
        public IOperation ReadRandomByteOperation => _readRandomByteOperation;

        public async Task<byte[]> ReadRandomBytesAsync(int bufferSize)
        {
            using (_readRandomBytesOperation.StartNew())
            {
                var readRandomByteTasks = new object[bufferSize].Select(_ => ReadRandomByteAsync()).ToArray();
                var buffer = await Task.WhenAll(readRandomByteTasks).ConfigureAwait(false);
                return buffer;
            }
        }

        private async Task<byte> ReadRandomByteAsync()
        {
            using (_readRandomByteOperation.StartNew())
            {
                var buffer = new byte[1];
                _random.NextBytes(buffer);

                await Task.Delay(TimeSpan.FromSeconds(_random.NextDouble())).ConfigureAwait(false);

                _randomMetric.RecordValue(_random.Next());
                _bytesReadCounter.Increment();

                return buffer[0];
            }
        }
    }
}
