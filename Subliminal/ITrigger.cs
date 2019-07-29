using System;

namespace Subliminal
{
    public interface ITrigger<TContext>
    {
        Guid TriggerId { get; }
        IObservable<ActivatedTrigger<TContext>> Activated { get; }
    }
}
