using System;
using System.Collections.Generic;
using System.Linq;

namespace EventScope.Logging.Serilog
{
    public class OperationTimer : IEventSource<OperationCompletedEventArgs>, IScope, IScopeSource
    {
        private readonly ISubscription _subscription;
        private readonly Scope _scope;
        private readonly ManualEventSource<ScopeStartedEventArgs> _scopeStarted;
        private readonly ManualEventSource<OperationCompletedEventArgs> _operationCompleted;

        public OperationTimer(ISubscription subscription)
        {
            _subscription = subscription;
            _scope = new Scope(Guid.NewGuid(), Scope.RootScope);
            _scopeStarted = new ManualEventSource<ScopeStartedEventArgs>();
            _operationCompleted = new ManualEventSource<OperationCompletedEventArgs>();
        }

        public Guid ScopeId => _scope.ScopeId;
        public IScope ParentScope => _scope.ParentScope;
        public TimeSpan Duration => _scope.Duration;
        public IEventSource<ScopeEndedEventArgs> ScopeEnded => _scope.ScopeEnded;

        public bool IsActive => _subscription.IsActive;
        public HashSet<IScope> ActiveScopes => _subscription.ActiveScopes;

        public void HandleEvent(object sender, ScopeStartedEventArgs eventArgs)
        {
            _subscription.HandleEvent(sender, eventArgs);
        }

        public void AddHandler(IEventHandler<ScopeStartedEventArgs> eventHandler)
        {
            _scopeStarted.AddHandler(eventHandler);
        }

        public void RemoveHandler(IEventHandler<ScopeStartedEventArgs> eventHandler)
        {
            _scopeStarted.RemoveHandler(eventHandler);
        }

        public void AddHandler(IEventHandler<OperationCompletedEventArgs> eventHandler)
        {
            _operationCompleted.AddHandler(eventHandler);
        }

        public void RemoveHandler(IEventHandler<OperationCompletedEventArgs> eventHandler)
        {
            _operationCompleted.AddHandler(eventHandler);
        }

        public void Start()
        {
            var newScopes = ActiveScopes.Select(activeScope => new Scope(Guid.NewGuid(), activeScope)).ToArray();

            void StopScopes(object sender, ScopeEndedEventArgs eventArgs)
            {
                var operationDuration = eventArgs.EndedScope.Duration;

                foreach (var scope in newScopes)
                {
                    scope.ScopeEnded.AddHandler(
                        new DelegateEventHandler<ScopeEndedEventArgs>((_, __) =>
                            _operationCompleted.RaiseEvent(sender, new OperationCompletedEventArgs(scope, operationDuration))));

                    scope.Stop();
                }
            }

            _scope.ScopeEnded.AddHandler(new DelegateEventHandler<ScopeEndedEventArgs>(StopScopes));

            foreach (var newScope in newScopes)
            {
                newScope.Start();
                _scopeStarted.RaiseEvent(this, new ScopeStartedEventArgs(newScope));
            }

            _scope.Start();
        }

        public void Stop()
        {
            _scope.Stop();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}
