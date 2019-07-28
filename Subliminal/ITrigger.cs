using System;

namespace Subliminal
{
    public interface ITrigger<TEvent> : IObservable<TEvent>
    {
        Guid TriggerId { get; }
    }
}
