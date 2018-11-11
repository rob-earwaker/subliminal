using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventScope.Logging.Serilog
{
    public class OperationDurationSummaryLogger : IEventHandler<OperationCompletedEventArgs>
    {
        private readonly string _operationName;
        private readonly ILogger _logger;
        private readonly LogEventLevel _logEventLevel;
        private readonly Dictionary<IScope, List<TimeSpan>> _operationDurations;
        private readonly object _operationDurationsLock;
        private readonly IEventHandler<ScopeEndedEventArgs> _scopeEndedHandler;

        public OperationDurationSummaryLogger(string operationName, ILogger logger, LogEventLevel logEventLevel)
        {
            _operationName = operationName;
            _logger = logger;
            _logEventLevel = logEventLevel;
            _operationDurations = new Dictionary<IScope, List<TimeSpan>>();
            _operationDurationsLock = new object();
            _scopeEndedHandler = new DelegateEventHandler<ScopeEndedEventArgs>(LogSummary);
        }

        public void HandleEvent(object sender, OperationCompletedEventArgs eventArgs)
        {
            lock (_operationDurationsLock)
            {
                if (!_operationDurations.TryGetValue(eventArgs.EventScope, out var operationDurations))
                {
                    _operationDurations.Add(eventArgs.EventScope, new List<TimeSpan> { eventArgs.OperationDuration });
                    eventArgs.EventScope.ScopeEnded.AddHandler(_scopeEndedHandler);
                }
                else
                {
                    operationDurations.Add(eventArgs.OperationDuration);
                }
            }
        }

        private void LogSummary(object sender, ScopeEndedEventArgs eventArgs)
        {
            List<TimeSpan> operationDurations;

            lock (_operationDurationsLock)
            {
                eventArgs.EventScope.ScopeEnded.RemoveHandler(_scopeEndedHandler);

                if (!_operationDurations.TryGetValue(eventArgs.EventScope, out operationDurations))
                    return;

                _operationDurations.Remove(eventArgs.EventScope);
            }

            var averageDurationSeconds = operationDurations?.Average(duration => duration.TotalSeconds) ?? 0;

            _logger.Write(
                _logEventLevel,
                "Average time taken to {OperationName} was {AverageDurationSeconds} over the last {SamplePeriodDurationSeconds}",
                _operationName, averageDurationSeconds, eventArgs.EventScope.Duration);
        }
    }
}
