using System;
using System.Collections.Generic;

namespace Subliminal
{
    public interface ISource<TValue>
    {
        IObservable<Sourced<TValue>> Values { get; }

        ISource<IList<Sourced<TValue>>> Buffer(int count, int skip);
        ISource<TNewValue> Select<TNewValue>(Func<TValue, TNewValue> selector);
    }
}
