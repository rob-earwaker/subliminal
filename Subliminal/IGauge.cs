using Subliminal.Events;
using System;
using System.Collections.Generic;

namespace Subliminal
{
    public interface IGauge<TValue>
    {
        IObservable<GaugeSampled<TValue>> Sampled { get; }
        IGauge<IList<TValue>> Buffer(int count, int skip);
        IGauge<TResult> Select<TResult>(Func<TValue, TResult> selector);
    }
}
