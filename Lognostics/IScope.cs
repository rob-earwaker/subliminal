using System;

namespace Lognostics
{
    public interface IScope : IDisposable
    {
        Guid Id { get; }
        TimeSpan Duration { get; }
        event EventHandler<ScopeStoppedEventArgs> Stopped;
        void Start();
        void Stop();
    }
}