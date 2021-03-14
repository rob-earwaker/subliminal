using System;
using System.Reactive.Subjects;

namespace Subliminal
{
    public class ObservableLog<TLogEntry> : IObservable<TLogEntry>
        where TLogEntry : ILogEntry
    {
        private readonly IObservable<TLogEntry> _observable;

        private ObservableLog(IObservable<TLogEntry> observable)
        {
            _observable = observable;
        }

        public static ObservableLog<TLogEntry> FromLog(ILog<TLogEntry> log)
        {
            var subject = new Subject<TLogEntry>();
            log.Subscribe(subject.OnNext);
            return new ObservableLog<TLogEntry>(subject);
        }

        public IDisposable Subscribe(IObserver<TLogEntry> observer)
        {
            return _observable.Subscribe(observer);
        }
    }
}
