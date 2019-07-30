using System;
using System.Linq;
using System.Threading.Tasks;

namespace Subliminal.Serilog.TestApp
{
    internal class DataStore
    {
        private readonly Random _random;
        private readonly Gauge<int> _randomGauge;
        private readonly Counter<ByteCount> _bytesReadCounter;
        private readonly Operation _readRandomBytesOperation;
        private readonly Operation _readRandomByteOperation;

        public DataStore()
        {
            _random = new Random();
            _randomGauge = new Gauge<int>();
            _bytesReadCounter = new Counter<ByteCount>();
            _readRandomBytesOperation = new Operation();
            _readRandomByteOperation = new Operation();
        }

        public IGauge<int> RandomGauge => _randomGauge;
        public ICounter<ByteCount> BytesReadCounter => _bytesReadCounter;
        public IOperation ReadRandomBytesOperation => _readRandomBytesOperation;
        public IOperation ReadRandomByteOperation => _readRandomByteOperation;

        public async Task<byte[]> ReadRandomBytesAsync(int bufferSize)
        {
            using (_readRandomBytesOperation.StartNewTimer())
            {
                var readRandomByteTasks = new object[bufferSize].Select(_ => ReadRandomByteAsync()).ToArray();
                var buffer = await Task.WhenAll(readRandomByteTasks).ConfigureAwait(false);
                return buffer;
            }
        }

        private async Task<byte> ReadRandomByteAsync()
        {
            using (_readRandomByteOperation.StartNewTimer())
            {
                var buffer = new byte[1];
                _random.NextBytes(buffer);

                await Task.Delay(TimeSpan.FromSeconds(_random.NextDouble())).ConfigureAwait(false);

                _randomGauge.RecordValue(_random.Next());
                _bytesReadCounter.IncrementBy(ByteCount.FromBytes(1));

                return buffer[0];
            }
        }
    }
}
