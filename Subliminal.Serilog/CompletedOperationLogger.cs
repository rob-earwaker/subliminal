using Serilog;
using Serilog.Events;
using System;

namespace Subliminal.Serilog
{
    public class CompletedOperationLogger : IObserver<CompletedOperation>
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

        public void OnNext(CompletedOperation operation)
        {
            _logger.ForContext("OperationId", operation.OperationId)
                .ForContext("DurationSeconds", operation.Duration.TotalSeconds)
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
