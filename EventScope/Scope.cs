using System;
using System.Diagnostics;

namespace EventScope
{
    public class Scope : IScope
    {
        public static readonly IScope RootScope = new Scope(Guid.NewGuid(), null);
        
        private readonly IScope _parentScope;
        private readonly ManualEventSource<ScopeEndedEventArgs> _scopeEnded;
        private readonly Stopwatch _stopwatch;
        private readonly object _controlStateLock;

        public Scope(Guid scopeId, IScope parentScope)
        {
            ScopeId = scopeId;
            _parentScope = parentScope;
            _scopeEnded = new ManualEventSource<ScopeEndedEventArgs>();
            _stopwatch = new Stopwatch();
            _controlStateLock = new object();
        }

        public Guid ScopeId { get; }
        public IScope ParentScope => _parentScope ?? RootScope;
        public TimeSpan Duration => _stopwatch.Elapsed;
        public IEventSource<ScopeEndedEventArgs> ScopeEnded => _scopeEnded;

        public void Start()
        {
            lock (_controlStateLock)
            {
                if (_stopwatch.IsRunning)
                    return;

                _stopwatch.Start();
            }
        }

        public void Stop()
        {
            lock (_controlStateLock)
            {
                if (!_stopwatch.IsRunning)
                    return;

                _stopwatch.Stop();
                _scopeEnded.RaiseEvent(this, new ScopeEndedEventArgs(this));
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
