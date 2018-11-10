using System;

namespace EventScope.Logging.Serilog
{
    public class Operation : IEventSource<OperationCompletedEventArgs>, IEventSource<ScopeStartedEventArgs>
    {
        private readonly ManualEventSource<ScopeStartedEventArgs> _scopeStarted;
        private readonly DelegateEventHandler<ScopeEndedEventArgs> _scopeEndedHandler;

        private event EventHandler<OperationCompletedEventArgs> _operationCompleted;

        public Operation()
        {
            _scopeStarted = new ManualEventSource<ScopeStartedEventArgs>();
            _scopeEndedHandler = new DelegateEventHandler<ScopeEndedEventArgs>(StopTimer);
        }
        
        public void AddHandler(IEventHandler<OperationCompletedEventArgs> eventHandler)
        {
            _operationCompleted += eventHandler.HandleEvent;
        }

        public void RemoveHandler(IEventHandler<OperationCompletedEventArgs> eventHandler)
        {
            _operationCompleted -= eventHandler.HandleEvent;
        }

        public void AddHandler(IEventHandler<ScopeStartedEventArgs> eventHandler)
        {
            _scopeStarted.AddHandler(eventHandler);
        }

        public void RemoveHandler(IEventHandler<ScopeStartedEventArgs> eventHandler)
        {
            _scopeStarted.RemoveHandler(eventHandler);
        }

        public IScope StartNewTimer()
        {
            var scope = new Scope(Guid.NewGuid());
            scope.ScopeEnded.AddHandler(_scopeEndedHandler);
            scope.Start();
            _scopeStarted.RaiseEvent(this, new ScopeStartedEventArgs(scope));
            return scope;
        }

        private void StopTimer(object sender, ScopeEndedEventArgs eventArgs)
        {
            _operationCompleted?.Invoke(this, new OperationCompletedEventArgs(eventArgs.Scope));
        }
    }
}
