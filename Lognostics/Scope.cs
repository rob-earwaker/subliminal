using Lognostics.Events;
using System;
using System.Diagnostics;

namespace Lognostics
{
    public class Scope : IScope
    {
        private readonly Stopwatch _stopwatch;

        public Scope(Guid scopeSourceId)
        {
            ScopeId = Guid.NewGuid();
            ScopeSourceId = scopeSourceId;
            HasStarted = false;
            HasEnded = false;
            _stopwatch = new Stopwatch();
        }

        public static Scope StartNew(Guid scopeSourceId)
        {
            var scope = new Scope(scopeSourceId);
            scope.Start();
            return scope;
        }

        public Guid ScopeId { get; }
        public Guid ScopeSourceId { get; }
        public bool HasStarted { get; private set; }
        public bool HasEnded { get; private set; }
        public TimeSpan Duration => _stopwatch.Elapsed;

        public event EventHandler<ScopeEnded> Ended;

        public void Start()
        {
            if (HasStarted)
                return;

            _stopwatch.Start();
            HasStarted = true;
        }

        public void End()
        {
            if (!HasStarted || HasEnded)
                return;

            _stopwatch.Stop();
            Ended?.Invoke(this, new ScopeEnded(this));
            HasEnded = true;
        }

        public void Dispose()
        {
            End();
        }
    }
}
