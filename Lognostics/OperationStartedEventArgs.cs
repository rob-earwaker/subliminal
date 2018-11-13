using System;

namespace Lognostics
{
    public class OperationStartedEventArgs : EventArgs
    {
        public OperationStartedEventArgs(Operation operationScope)
        {
            OperationScope = operationScope;
        }

        public Operation OperationScope { get; }
    }
}
