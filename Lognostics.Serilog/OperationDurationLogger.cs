using Serilog;
using Serilog.Events;

namespace Lognostics.Serilog
{
    public class OperationDurationLogger : IEventHandler<OperationCompletedEventArgs>
    {
        private readonly string _operationName;
        private readonly ILogger _logger;
        private readonly LogEventLevel _logEventLevel;

        public OperationDurationLogger(string operationName, ILogger logger, LogEventLevel logEventLevel = LogEventLevel.Information)
        {
            _operationName = operationName;
            _logger = logger;
            _logEventLevel = logEventLevel;
        }

        public void HandleEvent(object sender, OperationCompletedEventArgs eventArgs)
        {
            _logger.Write(
                _logEventLevel,
                "Took {OperationDurationSeconds}s to {OperationName}",
                eventArgs.OperationScope.Duration.TotalSeconds, _operationName);
        }
    }
}
