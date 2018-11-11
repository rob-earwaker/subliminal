using System;

namespace EventScope.Logging.Serilog
{
    public class OperationCompletedEventArgs : ScopeEndedEventArgs
    {
        public OperationCompletedEventArgs(IScope eventScope, TimeSpan operationDuration) : base(eventScope)
        {
            OperationDuration = operationDuration;
        }

        public TimeSpan OperationDuration { get; }
    }
}