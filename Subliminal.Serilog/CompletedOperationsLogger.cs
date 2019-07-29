using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;

namespace Subliminal.Serilog
{
    public class CompletedOperationsLogger : IObserver<TimeInterval<IList<Event<OperationCompleted>>>>
    {
        private readonly ILogger _logger;
        private readonly LogEventLevel _level;
        private readonly string _messageTemplate;

        public CompletedOperationsLogger(ILogger logger, LogEventLevel level, string messageTemplate)
        {
            _logger = logger;
            _level = level;
            _messageTemplate = messageTemplate;
        }

        public void OnNext(TimeInterval<IList<Event<OperationCompleted>>> buffer)
        {
            if (!buffer.Value.Any())
                return;

            var averageDurationSeconds = buffer.Value.Average(completed => completed.Context.Duration.TotalSeconds);

            _logger.ForContext("AverageDurationSeconds", averageDurationSeconds)
                .ForContext("SamplePeriodDurationSeconds", buffer.Interval.TotalSeconds)
                .Write(_level, _messageTemplate);
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }
    }
}
