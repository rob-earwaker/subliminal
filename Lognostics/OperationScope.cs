using System;

namespace Lognostics
{
    public class OperationScope : IScope
    {
        private readonly IScope _timerScope;

        public OperationScope(Guid operationTypeId)
        {
            OperationTypeId = operationTypeId;
            _timerScope = new Scope();
        }

        public static OperationScope StartNew(Guid operationTypeId)
        {
            var operationTimer = new OperationScope(operationTypeId);
            operationTimer.Start();
            return operationTimer;
        }
        
        public Guid OperationTypeId { get; }
        public Guid ScopeId => _timerScope.ScopeId;
        public bool IsStarted => _timerScope.IsStarted;
        public TimeSpan Duration => _timerScope.Duration;

        public event EventHandler<OperationCompletedEventArgs> Completed;
        public event EventHandler<ScopeEndedEventArgs> Ended;

        public void Start()
        {
            if (IsStarted)
                return;

            _timerScope.Ended += TimerScopeEndedHandler;
            _timerScope.Start();
        }

        public void End()
        {
            _timerScope.End();
        }

        public void Dispose()
        {
            _timerScope.Dispose();
        }

        private void TimerScopeEndedHandler(object sender, ScopeEndedEventArgs eventArgs)
        {
            eventArgs.Scope.Ended -= TimerScopeEndedHandler;
            Ended?.Invoke(this, new ScopeEndedEventArgs(this));
            Completed?.Invoke(this, new OperationCompletedEventArgs(this));
        }
    }
}
