using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventScope.Test
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            var dataStore = new DataStore();

            for (var index = 0; index < 8; index++)
            {
                var buffer = await dataStore.ReadRandomBytesAsync(16).ConfigureAwait(false);
                var hexString = string.Concat(buffer.Select(b => $"{b:X2}"));
                Log.Information("Read random bytes from data store: {BytesHex}", hexString);
            }

            Log.Information("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
