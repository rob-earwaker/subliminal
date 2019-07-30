using System.Collections.Generic;
using System.Linq;

namespace Subliminal
{
    public static class IEnumerableExtensions
    {
        public static Rate Average(this IEnumerable<Rate> rates)
        {
            return Rate.Average(rates);
        }

        public static BitRate Average(this IEnumerable<BitRate> bitRates)
        {
            return BitRate.Average(bitRates);
        }

        public static ByteCount Sum(this IEnumerable<ByteCount> byteCounts)
        {
            return byteCounts.Aggregate(ByteCount.Zero, (total, byteCount) => total + byteCount);
        }
    }
}
