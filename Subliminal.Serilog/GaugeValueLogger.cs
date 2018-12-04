using Serilog;
using Serilog.Events;
using System;

namespace Subliminal.Serilog
{
    public class GaugeValueLogger<TGauge> : IObserver<TGauge>
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

        public void OnNext(TGauge value)
        {
            _logger.ForContext("Value", value)
                .Write(_logEventLevel, _messageTemplate);
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }
    }
}
