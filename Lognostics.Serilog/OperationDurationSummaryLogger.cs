using Serilog;
using System.Linq;

namespace Lognostics.Serilog
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
                "Average time taken to {OperationName} was {AverageDurationSeconds}s over the last {SamplePeriodDurationSeconds}s",
                _operationName,
                eventArgs.Value.Average(operationCompletedEventArgs => operationCompletedEventArgs.Operation.Duration.TotalSeconds),
                eventArgs.Scope.Duration.TotalSeconds);
        }
    }
}
