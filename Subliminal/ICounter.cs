using System;

namespace Subliminal
{
    public interface ICounter : IObservable<int>
    {
        Guid CounterId { get; }
    }
}
