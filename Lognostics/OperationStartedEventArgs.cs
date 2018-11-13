using System;

namespace Lognostics
{
    public class OperationStartedEventArgs : EventArgs
    {
        public OperationStartedEventArgs(OperationScope operationScope)
        {
            Operation = operationScope;
        }

        public OperationScope Operation { get; }
    }
}
