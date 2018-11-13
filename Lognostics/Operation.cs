using System;
using System.Diagnostics;

namespace Lognostics
{
    public class Operation : IScope
    {
        private readonly IScope _timerScope;

        public Operation(IScope timerScope)
        {
            _timerScope = timerScope;
        }

        public static Operation StartNew()
        {
            var timerScope = new Scope(Guid.NewGuid(), new Stopwatch());
            var operationTimer = new Operation(timerScope);
            operationTimer.Start();
            return operationTimer;
        }

        public Guid Id => _timerScope.Id;

        public bool IsStarted => _timerScope.IsStarted;

        public TimeSpan Duration => _timerScope.Duration;

        public event EventHandler<ScopeEndedEventArgs> Ended;

        public void Start()
        {
            if (IsStarted)
                return;

            _timerScope.Ended += TimerScopeEndedHandler;
            _timerScope.Start();
        }

        public void Stop()
        {
            _timerScope.Stop();
        }

        public void Dispose()
        {
            _timerScope.Dispose();
        }

        private void TimerScopeEndedHandler(object sender, ScopeEndedEventArgs eventArgs)
        {
            eventArgs.Scope.Ended -= TimerScopeEndedHandler;
            Ended?.Invoke(this, new ScopeEndedEventArgs(this));
        }
    }
}
