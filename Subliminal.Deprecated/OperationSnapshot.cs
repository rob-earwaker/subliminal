using System;

namespace Subliminal
{
    public class OperationSnapshot
    {
        public OperationSnapshot(
            Guid operationId, TimeSpan currentDuration, Context context, OperationSnapshot parent)
        {
            OperationId = operationId;
            CurrentDuration = currentDuration;
            Context = context;
            Parent = parent;
        }

        public Guid OperationId { get; }
        public TimeSpan CurrentDuration { get; }
        public Context Context { get; }
        public OperationSnapshot Parent { get; }
    }
}
