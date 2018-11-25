using Lognostics.Events;
using System;
using System.Collections.Generic;

namespace Lognostics
{
    public interface IScope : IDisposable
    {        
        Guid ScopeId { get; }
        Guid ScopeSourceId { get; }
        bool HasStarted { get; }
        bool HasEnded { get; }
        IReadOnlyDictionary<string, object> Context { get; }
        TimeSpan Duration { get; }
        event EventHandler<ScopeEnded> Ended;
        void Start();
        void End();
        void AddContext(string contextKey, object value);
    }
}