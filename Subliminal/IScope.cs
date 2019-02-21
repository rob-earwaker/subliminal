using System;
using System.Collections.Generic;
using System.Reactive;

namespace Subliminal
{
    public interface IScope : IDisposable
    {
        bool HasStarted { get; }
        bool HasEnded { get; }
        IReadOnlyDictionary<string, object> Context { get; }
        TimeSpan Duration { get; }
        IObservable<Unit> Ended { get; }
        void Start();
        void End();
        void AddContext(string contextKey, object value);
    }
}