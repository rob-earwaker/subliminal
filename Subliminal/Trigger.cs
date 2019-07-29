using System;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class Trigger<TContext> : ITrigger<TContext>
    {
        private readonly Subject<TContext> _eventSubject;
        private readonly ITrigger<TContext> _derivedTrigger;

        public Trigger()
        {
            _eventSubject = new Subject<TContext>();
            _derivedTrigger = _eventSubject.AsTrigger();
        }

        public void Raise(TContext @event)
        {
            _eventSubject.OnNext(@event);
            _eventSubject.OnCompleted();
        }

        public Guid TriggerId => _derivedTrigger.TriggerId;
        public IObservable<ActivatedTrigger<TContext>> Activated => _derivedTrigger.Activated;
    }
}
