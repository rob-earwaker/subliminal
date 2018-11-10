using System;

namespace EventScope.Logging.Serilog
{
    public class OperationCompletedEventArgs : ScopeEndedEventArgs
    {
        public OperationCompletedEventArgs(IScope scope) : base(scope)
        {
        }

        public TimeSpan OperationDuration => Scope.Duration;
    }
}