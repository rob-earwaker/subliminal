using System;

namespace Subliminal
{
    public interface ITimer : IObservable<TimeSpan>
    {
        Guid TimerId { get; }
    }
}
