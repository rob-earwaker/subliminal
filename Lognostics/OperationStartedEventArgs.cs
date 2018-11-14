using System;

namespace Lognostics
{
    public class OperationStartedEventArgs : EventArgs
    {
        public OperationStartedEventArgs(OperationScope operationScope)
        {
            OperationScope = operationScope;
        }

        public OperationScope OperationScope { get; }
    }
}
