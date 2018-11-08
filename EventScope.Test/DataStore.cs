using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventScope.Test
{
    internal class DataStore
    {
        private readonly Random _random;

        public DataStore()
        {
            _random = new Random();
        }

        public async Task<byte[]> ReadRandomBytesAsync(int size)
        {
            return await Task.WhenAll(new object[size].Select(_ => ReadRandomByteAsync()));
        }

        private async Task<byte> ReadRandomByteAsync()
        {
            var buffer = new byte[1];
            _random.NextBytes(buffer);
            await Task.Delay(TimeSpan.FromSeconds(_random.NextDouble())).ConfigureAwait(false);
            return buffer[0];
        }
    }
}
