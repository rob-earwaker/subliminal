using System;
using System.Collections.Generic;

namespace Subliminal
{
    public interface IGauge<TValue>
    {
        IObservable<Sample<TValue>> Sampled { get; }
        IGauge<IList<Sample<TValue>>> Buffer(int count, int skip);
        IGauge<TResult> Select<TResult>(Func<TValue, TResult> selector);
    }
}
