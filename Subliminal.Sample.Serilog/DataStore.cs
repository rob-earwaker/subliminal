using System;
using System.Linq;
using System.Threading.Tasks;

namespace Subliminal.Sample.Serilog
{
    internal class DataStore
    {
        private static readonly Random _random = new Random();
        private static readonly object _randomLockObject = new object();

        private static readonly Gauge<int> _randomGauge = new Gauge<int>();
        private static readonly Counter<long> _bytesReadCounter = new Counter<long>();
        private static readonly Operation _readRandomBytesOperation = new Operation();
        private static readonly Operation _readRandomByteOperation = new Operation();
        
        public static IGauge<int> RandomGauge => _randomGauge;
        public static ICounter<long> BytesReadCounter => _bytesReadCounter;
        public static IOperation ReadRandomBytesOperation => _readRandomBytesOperation;
        public static IOperation ReadRandomByteOperation => _readRandomByteOperation;

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

                _randomGauge.LogValue(randomValue);
                _bytesReadCounter.IncrementBy(1);

                return buffer[0];
            }
        }
    }
}
