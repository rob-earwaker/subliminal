using System;

namespace Lognostics
{
    public class OperationCompletedEventArgs : EventArgs
    {
        public OperationCompletedEventArgs(OperationScope operationScope)
        {
            OperationScope = operationScope;
        }

        public OperationScope OperationScope { get; }
    }
}