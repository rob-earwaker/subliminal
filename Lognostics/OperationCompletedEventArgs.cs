﻿using System;

namespace Lognostics
{
    public class OperationCompletedEventArgs : EventArgs
    {
        public OperationCompletedEventArgs(TimeSpan operationDuration)
        {
            OperationDuration = operationDuration;
        }

        public TimeSpan OperationDuration { get; }
    }
}