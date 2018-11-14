using System;

namespace Lognostics
{
    public interface IScope : IDisposable
    {
        Guid ScopeId { get; }
        bool HasStarted { get; }
        bool HasEnded { get; }
        TimeSpan Duration { get; }
        event EventHandler<ScopeEndedEventArgs> Ended;
        void Start();
        void End();
    }
}