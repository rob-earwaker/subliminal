using System;
using System.Diagnostics;

namespace Lognostics
{
    public class Scope : IScope
    {
        private readonly Stopwatch _stopwatch;

        public Scope()
        {
            ScopeId = Guid.NewGuid();
            IsStarted = false;
            _stopwatch = new Stopwatch();
        }

        public static Scope StartNew()
        {
            var scope = new Scope();
            scope.Start();
            return scope;
        }

        public Guid ScopeId { get; }

        public bool IsStarted { get; private set; }

        public TimeSpan Duration => _stopwatch.Elapsed;

        public event EventHandler<ScopeEndedEventArgs> Ended;

        public void Start()
        {
            IsStarted = true;
            _stopwatch.Start();
        }

        public void End()
        {
            if (!IsStarted)
                return;

            _stopwatch.Stop();
            Ended?.Invoke(this, new ScopeEndedEventArgs(this));
        }

        public void Dispose()
        {
            End();
        }
    }
}
