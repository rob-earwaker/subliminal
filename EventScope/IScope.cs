using System;

namespace EventScope
{
    public interface IScope : IDisposable
    {
        Guid ScopeId { get; }
        TimeSpan Duration { get; }
        void Start();
        void Stop();
        IEventSource<ScopeEndedEventArgs> ScopeEnded { get; }
    }
}