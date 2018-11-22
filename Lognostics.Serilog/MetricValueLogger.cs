﻿using Lognostics.Events;
using Serilog;
using Serilog.Events;

namespace Lognostics.Serilog
{
    public class MetricValueLogger<TMetric> : IEventHandler<MetricSampled<TMetric>>
    {
        private readonly string _metricName;
        private readonly string _messageTemplate;
        private readonly ILogger _logger;
        private readonly LogEventLevel _logEventLevel;

        public MetricValueLogger(
            string metricName, string messageTemplate, ILogger logger, LogEventLevel logEventLevel = LogEventLevel.Information)
        {
            _metricName = metricName;
            _messageTemplate = messageTemplate;
            _logger = logger;
            _logEventLevel = logEventLevel;
        }

        public void HandleEvent(object sender, MetricSampled<TMetric> eventArgs)
        {
            _logger.ForContext("MetricId", eventArgs.MetricId)
                .ForContext("MetricName", _metricName)
                .ForContext("Value", eventArgs.Value)
                .Write(_logEventLevel, _messageTemplate);
        }
    }
}