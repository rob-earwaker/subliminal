using System;
using System.Linq;
using System.Reactive.Linq;

namespace Subliminal
{
    public static class ICounterExtensions
    {
        public static IObservable<BitRate> Rate(this ICounter<ByteCount> counter)
        {
            return counter
                .TimeInterval()
                .Buffer(count: 2, skip: 1)
                .Select(buffer => new BitRate(
                    count: buffer.Select(byteCount => byteCount.Value).Sum(),
                    interval: buffer[1].Interval));
        }
    }
}
