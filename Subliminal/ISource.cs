using System;
using System.Collections.Generic;

namespace Subliminal
{
    internal interface ISource<TValue>
    {
        IObservable<SourcedValue<TValue>> Values { get; }

        ISource<IList<SourcedValue<TValue>>> Buffer(int count, int skip);
        ISource<TNewValue> Select<TNewValue>(Func<TValue, TNewValue> selector);
    }
}
