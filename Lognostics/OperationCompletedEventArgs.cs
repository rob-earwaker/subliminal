using System;

namespace Lognostics
{
    public class OperationCompletedEventArgs : EventArgs
    {
        public OperationCompletedEventArgs(OperationScope operationScope)
        {
            Operation = operationScope;
        }

        public OperationScope Operation { get; }
    }
}