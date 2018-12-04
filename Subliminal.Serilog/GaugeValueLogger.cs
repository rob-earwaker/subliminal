using Serilog;
using Serilog.Events;
using Subliminal.Events;

namespace Subliminal.Serilog
{
    public class GaugeValueLogger<TGauge> : IEventHandler<GaugeSampled<TGauge>>
    {
        private readonly string _messageTemplate;
        private readonly ILogger _logger;
        private readonly LogEventLevel _logEventLevel;

        public GaugeValueLogger(string messageTemplate, ILogger logger, LogEventLevel logEventLevel)
        {
            _messageTemplate = messageTemplate;
            _logger = logger;
            _logEventLevel = logEventLevel;
        }

        public void HandleEvent(object sender, GaugeSampled<TGauge> eventArgs)
        {
            _logger.ForContext("Value", eventArgs.Value)
                .Write(_logEventLevel, _messageTemplate);
        }
    }
}
