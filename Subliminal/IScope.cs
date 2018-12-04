using Subliminal.Events;
using System;
using System.Collections.Generic;

namespace Subliminal
{
    public interface IScope : IDisposable
    {
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