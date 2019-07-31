using System;
using System.Linq;
using System.Threading.Tasks;

namespace Subliminal.Serilog.TestApp
{
    internal class DataStore
    {
        private readonly Random _random;
        private readonly object _randomLockObject;
        private readonly Gauge<int> _randomGauge;
        private readonly Counter<ByteCount> _bytesReadCounter;
        private readonly Operation _readRandomBytesOperation;
        private readonly Operation _readRandomByteOperation;

        public DataStore()
        {
            _random = new Random();
            _randomLockObject = new object();
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
                double randomDelay;
                int randomValue;

                lock (_randomLockObject)
                {
                    _random.NextBytes(buffer);
                    randomDelay = _random.NextDouble();
                    randomValue = _random.Next();
                }

                await Task.Delay(TimeSpan.FromSeconds(randomDelay)).ConfigureAwait(false);

                _randomGauge.RecordValue(randomValue);
                _bytesReadCounter.IncrementBy(ByteCount.FromBytes(1));

                return buffer[0];
            }
        }
    }
}
