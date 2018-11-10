using EventScope.Logging.Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventScope.Test
{
    internal class DataStore
    {
        private readonly Random _random;

        public Operation ReadRandomBytesOperation { get; }
        public Operation ReadRandomByteOperation { get; }

        public DataStore()
        {
            _random = new Random();
            ReadRandomBytesOperation = new Operation();
            ReadRandomByteOperation = new Operation();
        }

        public async Task<byte[]> ReadRandomBytesAsync(int size)
        {
            using (ReadRandomBytesOperation.StartNewTimer())
            {
                return await Task.WhenAll(new object[size].Select(_ => ReadRandomByteAsync()));
            }
        }

        private async Task<byte> ReadRandomByteAsync()
        {
            using (ReadRandomByteOperation.StartNewTimer())
            {
                var buffer = new byte[1];
                _random.NextBytes(buffer);
                await Task.Delay(TimeSpan.FromSeconds(_random.NextDouble())).ConfigureAwait(false);
                return buffer[0];
            }
        }
    }
}
