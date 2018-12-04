using Subliminal.Events;
using Serilog;
using Serilog.Events;
using System.Linq;

namespace Subliminal.Serilog
{
    public class OperationDurationSummaryLogger : IScopedEventHandler<OperationCompleted[]>
    {
        private readonly string _messageTemplate;
        private readonly LogEventLevel _logEventLevel;
        private readonly ILogger _logger;

        public OperationDurationSummaryLogger(string messageTemplate, ILogger logger, LogEventLevel logEventLevel)
        {
            _messageTemplate = messageTemplate;
            _logEventLevel = logEventLevel;
            _logger = logger;
        }

        public void HandleEvent(object sender, Scoped<OperationCompleted[]> eventArgs)
        {
            var averageDurationSeconds = eventArgs.Value
                .Average(completedEventArgs => completedEventArgs.OperationScope.Duration.TotalSeconds);

            var samplePeriodDurationSeconds = eventArgs.Scope.Duration.TotalSeconds;

            _logger.ForContext(new DictionaryEnricher(eventArgs.Scope.Context))
                .ForContext("AverageDurationSeconds", averageDurationSeconds)
                .ForContext("SamplePeriodDurationSeconds", samplePeriodDurationSeconds)
                .Write(_logEventLevel, _messageTemplate);
        }
    }
}
