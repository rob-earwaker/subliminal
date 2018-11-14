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
            HasStarted = false;
            HasEnded = false;
            _stopwatch = new Stopwatch();
        }

        public static Scope StartNew()
        {
            var scope = new Scope();
            scope.Start();
            return scope;
        }

        public Guid ScopeId { get; }
        public bool HasStarted { get; private set; }
        public bool HasEnded { get; private set; }
        public TimeSpan Duration => _stopwatch.Elapsed;

        public event EventHandler<ScopeEndedEventArgs> Ended;

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
            Ended?.Invoke(this, new ScopeEndedEventArgs(this));
            HasEnded = true;
        }

        public void Dispose()
        {
            End();
        }
    }
}
