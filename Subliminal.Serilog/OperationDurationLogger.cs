using Subliminal.Events;
using Serilog;
using Serilog.Events;

namespace Subliminal.Serilog
{
    public class OperationDurationLogger : IEventHandler<OperationCompleted>
    {
        private readonly string _messageTemplate;
        private readonly ILogger _logger;
        private readonly LogEventLevel _logEventLevel;

        public OperationDurationLogger(string messageTemplate, ILogger logger, LogEventLevel logEventLevel)
        {
            _messageTemplate = messageTemplate;
            _logger = logger;
            _logEventLevel = logEventLevel;
        }

        public void HandleEvent(object sender, OperationCompleted eventArgs)
        {
            _logger.ForContext(new DictionaryEnricher(eventArgs.OperationScope.Context))
                .ForContext("OperationDurationSeconds", eventArgs.OperationScope.Duration.TotalSeconds)
                .Write(_logEventLevel, _messageTemplate);
        }
    }
}
