using Lognostics.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lognostics
{
    public class Scope : IScope
    {
        private readonly Stopwatch _stopwatch;
        private readonly ConcurrentDictionary<string, object> _context;

        public Scope(Guid scopeSourceId)
        {
            ScopeId = Guid.NewGuid();
            ScopeSourceId = scopeSourceId;
            HasStarted = false;
            HasEnded = false;
            _stopwatch = new Stopwatch();
            _context = new ConcurrentDictionary<string, object>();
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
        public IReadOnlyDictionary<string, object> Context => _context;
        public TimeSpan Duration => _stopwatch.Elapsed;
        public bool HasEnded { get; private set; }

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

        public void AddContext(string contextKey, object value)
        {
            _context.AddOrUpdate(contextKey, value, (_, __) => value);
        }

        public void Dispose()
        {
            End();
        }
    }
}
