using System;
using System.Reactive;

namespace Subliminal
{
    public interface IScope : IDisposable
    {
        bool HasStarted { get; }
        bool HasEnded { get; }
        TimeSpan Duration { get; }
        IObservable<Unit> Ended { get; }
        void Start();
        void End();
    }
}