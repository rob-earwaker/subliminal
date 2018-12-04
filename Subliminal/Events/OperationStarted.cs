using System;

namespace Subliminal.Events
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
