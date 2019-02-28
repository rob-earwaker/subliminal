using System;
using System.Collections.Generic;

namespace Subliminal
{
    public interface ISource<TValue>
    {
        IObservable<Observation<TValue>> Observations { get; }
        ISource<IList<Observation<TValue>>> Buffer(int count, int skip);
        ISource<TNewValue> Select<TNewValue>(Func<TValue, TNewValue> selector);
    }
}
