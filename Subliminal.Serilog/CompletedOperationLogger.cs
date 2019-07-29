using Serilog;
using Serilog.Events;
using System;

namespace Subliminal.Serilog
{
    public class CompletedOperationLogger : IObserver<Event<OperationCompleted>>
    {
        private readonly ILogger _logger;
        private readonly LogEventLevel _level;
        private readonly string _messageTemplate;

        public CompletedOperationLogger(ILogger logger, LogEventLevel level, string messageTemplate)
        {
            _logger = logger;
            _level = level;
            _messageTemplate = messageTemplate;
        }

        public void OnNext(Event<OperationCompleted> completed)
        {
            _logger.ForContext("OperationId", completed.Context.OperationId)
                .ForContext("ExecutionId", completed.Context.ExecutionId)
                .ForContext("DurationSeconds", completed.Context.Duration.TotalSeconds)
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
