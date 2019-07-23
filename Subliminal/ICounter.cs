using System;

namespace Subliminal
{
    public interface ICounter : IObservable<long>
    {
        Guid CounterId { get; }
    }
}
