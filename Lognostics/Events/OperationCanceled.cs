using System;

namespace Lognostics.Events
{
    public class OperationCanceled : EventArgs
    {
        public OperationCanceled(OperationScope operationScope)
        {
            OperationScope = operationScope;
        }

        public OperationScope OperationScope { get; }
    }
}
