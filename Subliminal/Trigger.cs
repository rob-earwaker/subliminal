﻿using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class Trigger<TEvent> : ITrigger<TEvent>
    {
        private readonly Subject<TEvent> _eventSubject;
        private readonly ITrigger<TEvent> _derivedEvent;

        public Trigger()
        {
            _eventSubject = new Subject<TEvent>();
            _derivedEvent = _eventSubject.AsObservable().AsEvent();
        }

        public void Raise(TEvent @event)
        {
            _eventSubject.OnNext(@event);
            _eventSubject.OnCompleted();
        }

        public Guid TriggerId => _derivedEvent.TriggerId;

        public IDisposable Subscribe(IObserver<TEvent> observer)
        {
            return _derivedEvent.Subscribe(observer);
        }
    }
}
