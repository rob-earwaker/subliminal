using Subliminal.Events;
using System;

namespace Subliminal
{
    public interface IGauge<TValue>
    {
        IObservable<GaugeSampled<TValue>> Sampled { get; }
        IGauge<TResult> Select<TResult>(Func<TValue, TResult> selector);
    }
}
