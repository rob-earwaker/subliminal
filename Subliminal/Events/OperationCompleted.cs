using System;

namespace Subliminal.Events
{
    public class OperationCompleted : EventArgs
    {
        public OperationCompleted(OperationScope operationScope)
        {
            OperationScope = operationScope;
        }

        public OperationScope OperationScope { get; }
    }
}