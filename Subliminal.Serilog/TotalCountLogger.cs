using Subliminal.Events;
using Serilog;
using Serilog.Events;
using System.Linq;

namespace Subliminal.Serilog
{
    public class TotalCountLogger : IScopedEventHandler<CounterIncremented[]>
    {
        private readonly string _messageTemplate;
        private readonly ILogger _logger;
        private readonly LogEventLevel _logEventLevel;

        public TotalCountLogger(string messageTemplate, ILogger logger, LogEventLevel logEventLevel)
        {
            _messageTemplate = messageTemplate;
            _logger = logger;
            _logEventLevel = logEventLevel;
        }

        public void HandleEvent(object sender, Scoped<CounterIncremented[]> eventArgs)
        {
            _logger.ForContext(new DictionaryEnricher(eventArgs.Scope.Context))
                .ForContext("SamplePeriodDurationSeconds", eventArgs.Scope.Duration.TotalSeconds)
                .ForContext("TotalCount", eventArgs.Value.Sum(counterIncremented => counterIncremented.Increment))
                .Write(_logEventLevel, _messageTemplate);
        }
    }
}
