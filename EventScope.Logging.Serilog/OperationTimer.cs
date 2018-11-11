using System;
using System.Collections.Generic;

namespace EventScope.Logging.Serilog
{
    public class OperationTimer : IEventSource<OperationCompletedEventArgs>, IScopeSource, IDisposable
    {
        private readonly Scope _scope;
        private readonly ManualScopeSource _scopeSource;
        private readonly ManualEventSource<OperationCompletedEventArgs> _operationCompleted;

        public OperationTimer(ISubscription subscription)
        {
            _scope = new Scope(Guid.NewGuid(), Scope.RootScope);
            _scopeSource = new ManualScopeSource(subscription);
            _operationCompleted = new ManualEventSource<OperationCompletedEventArgs>();
        }

        public bool IsActive => _scopeSource.IsActive;
        public HashSet<IScope> ActiveScopes => _scopeSource.ActiveScopes;

        public void HandleEvent(object sender, ScopeStartedEventArgs eventArgs)
        {
            _scopeSource.HandleEvent(sender, eventArgs);
        }

        public void AddSubscription(ISubscription subscription)
        {
            _scopeSource.AddSubscription(subscription);
        }

        public void RemoveSubscription(ISubscription subscription)
        {
            _scopeSource.RemoveSubscription(subscription);
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
            var newScopes = _scopeSource.StartNewScopes();

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
