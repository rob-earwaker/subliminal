using Lognostics.Events;
using System;

namespace Lognostics
{
    public class OperationScope : IScope
    {
        private readonly IScope _timerScope;

        public OperationScope(Guid operationTypeId)
        {
            OperationTypeId = operationTypeId;
            _timerScope = new Scope(ScopeSourceId);
        }

        public static OperationScope StartNew(Guid operationTypeId)
        {
            var operationTimer = new OperationScope(operationTypeId);
            operationTimer.Start();
            return operationTimer;
        }
        
        public Guid OperationTypeId { get; }

        public Guid ScopeId => _timerScope.ScopeId;
        public Guid ScopeSourceId => OperationTypeId;
        public bool HasStarted => _timerScope.HasStarted;
        public bool HasEnded => _timerScope.HasEnded;
        public TimeSpan Duration => _timerScope.Duration;
        
        public event EventHandler<OperationCompleted> Completed;
        public event EventHandler<ScopeEnded> Ended;

        public void Start()
        {
            if (HasStarted)
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

        private void TimerScopeEndedHandler(object sender, ScopeEnded eventArgs)
        {
            eventArgs.Scope.Ended -= TimerScopeEndedHandler;
            Ended?.Invoke(this, new ScopeEnded(this));
            Completed?.Invoke(this, new OperationCompleted(this));
        }
    }
}
