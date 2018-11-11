using Serilog;
using System.Linq;

namespace EventScope.Logging.Serilog
{
    public class OperationDurationSummaryLogger : IScopedEventHandler<OperationCompletedEventArgs[]>
    {
        private readonly string _operationName;
        private readonly ILogger _logger;

        public OperationDurationSummaryLogger(string operationName, ILogger logger)
        {
            _operationName = operationName;
            _logger = logger;
        }

        public void HandleEvent(object sender, ScopedEventArgs<OperationCompletedEventArgs[]> eventArgs)
        {
            _logger.Information(
                "Average time taken to {OperationName} was {AverageDurationSeconds}s over the last {SamplePeriodDurationSeconds}s within scope {ScopeId}",
                _operationName,
                eventArgs.Value.Average(operationCompletedEventArgs => operationCompletedEventArgs.OperationDuration.TotalSeconds),
                eventArgs.Scope.Duration.TotalSeconds,
                $"{eventArgs.Scope.GetHashCode():X8}");
        }
    }
}
