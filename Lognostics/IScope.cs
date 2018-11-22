using Lognostics.Events;
using System;

namespace Lognostics
{
    public interface IScope : IDisposable
    {        
        Guid ScopeId { get; }
        Guid ScopeSourceId { get; }
        bool HasStarted { get; }
        bool HasEnded { get; }
        TimeSpan Duration { get; }
        event EventHandler<ScopeEnded> Ended;
        void Start();
        void End();
    }
}