using System.Collections.Generic;

namespace Subliminal
{
    public static class IEnumerableExtensions
    {
        public static RateOfChange Average(this IEnumerable<RateOfChange> ratesOfChange)
        {
            return RateOfChange.Average(ratesOfChange);
        }
    }
}
