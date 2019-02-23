﻿using Serilog;
using Serilog.Events;
using Subliminal.Events;
using System;

namespace Subliminal.Serilog
{
    public class OperationCompletedLogger : IObserver<OperationCompleted>
    {
        private readonly ILogger _logger;
        private readonly LogEventLevel _level;
        private readonly string _messageTemplate;

        public OperationCompletedLogger(ILogger logger, LogEventLevel level, string messageTemplate)
        {
            _logger = logger;
            _level = level;
            _messageTemplate = messageTemplate;
        }

        public void OnNext(OperationCompleted completed)
        {
            _logger.ForContext("DurationSeconds", completed.Operation.Duration.TotalSeconds)
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
