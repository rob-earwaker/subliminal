using System;
using System.Collections.Generic;

namespace Subliminal
{
    public interface ISampleSource<TValue>
    {
        IObservable<Sample<TValue>> Samples { get; }

        ISampleSource<IList<Sample<TValue>>> Buffer(int count, int skip);
        ISampleSource<TNewValue> Select<TNewValue>(Func<TValue, TNewValue> selector);
    }
}
