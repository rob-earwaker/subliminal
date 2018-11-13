using System;
using System.Diagnostics;

namespace Lognostics
{
    public class Scope : IScope
    {
        private readonly Stopwatch _stopwatch;

        public Scope(Guid scopeId, Stopwatch stopwatch)
        {
            _stopwatch = stopwatch;
            Id = scopeId;
            IsStarted = false;
        }

        public static Scope StartNew()
        {
            var scope = new Scope(Guid.NewGuid(), new Stopwatch());
            scope.Start();
            return scope;
        }

        public Guid Id { get; }

        public bool IsStarted { get; private set; }

        public TimeSpan Duration => _stopwatch.Elapsed;

        public event EventHandler<ScopeEndedEventArgs> Ended;

        public void Start()
        {
            IsStarted = true;
            _stopwatch.Start();
        }

        public void Stop()
        {
            if (!IsStarted)
                return;

            _stopwatch.Stop();
            Ended?.Invoke(this, new ScopeEndedEventArgs(this));
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
