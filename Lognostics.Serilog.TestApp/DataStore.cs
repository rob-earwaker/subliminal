using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lognostics.Serilog.TestApp
{
    internal class DataStore
    {
        private readonly Random _random;

        public OperationHandle ReadRandomBytesOperation { get; }
        public OperationHandle ReadRandomByteOperation { get; }

        public DataStore()
        {
            _random = new Random();
            ReadRandomBytesOperation = new OperationHandle();
            ReadRandomByteOperation = new OperationHandle();
        }

        public async Task<byte[]> ReadRandomBytesAsync(int size)
        {
            using (ReadRandomBytesOperation.StartNew())
            {
                return await Task.WhenAll(new object[size].Select(_ => ReadRandomByteAsync()));
            }
        }

        private async Task<byte> ReadRandomByteAsync()
        {
            using (ReadRandomByteOperation.StartNew())
            {
                var buffer = new byte[1];
                _random.NextBytes(buffer);
                await Task.Delay(TimeSpan.FromSeconds(_random.NextDouble())).ConfigureAwait(false);
                return buffer[0];
            }
        }
    }
}
