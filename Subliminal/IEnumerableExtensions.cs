using System.Collections.Generic;

namespace Subliminal
{
    public static class IEnumerableExtensions
    {
        public static Rate Average(this IEnumerable<Rate> rates)
        {
            return Rate.Average(rates);
        }
    }
}
