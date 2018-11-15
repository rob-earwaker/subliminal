using System;

namespace Lognostics.Events
{
    public class OperationStarted : EventArgs
    {
        public OperationStarted(OperationScope operationScope)
        {
            OperationScope = operationScope;
        }

        public OperationScope OperationScope { get; }
    }
}
