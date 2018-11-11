using System;
using System.Diagnostics;

namespace Lognostics
{
    public class Scope : IScope
    {
        private readonly Stopwatch _stopwatch;
        private readonly object _controlStateLock;

        public Scope(Guid scopeId, Stopwatch stopwatch)
        {
            Id = scopeId;
            _stopwatch = stopwatch;
            _controlStateLock = new object();
        }

        public static Scope StartNew()
        {
            var scope = new Scope(Guid.NewGuid(), new Stopwatch());
            scope.Start();
            return scope;
        }

        public Guid Id { get; }

        public TimeSpan Duration => _stopwatch.Elapsed;

        public event EventHandler<ScopeStoppedEventArgs> Stopped;

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
                Stopped?.Invoke(this, new ScopeStoppedEventArgs(this));
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
