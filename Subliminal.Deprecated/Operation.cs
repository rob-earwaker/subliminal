using System;
using System.Diagnostics;
using System.Threading;

namespace Subliminal
{
    internal class Operation
    {
        private static readonly AsyncLocal<Operation> CurrentOperation = new AsyncLocal<Operation>();

        private Operation(Guid operationId, Stopwatch stopwatch, Context context, Operation parent)
        {
            OperationId = operationId;
            Stopwatch = stopwatch;
            Context = context;
            Parent = parent;
        }

        public static Operation StartNew(Context context)
        {
            var operationId = Guid.NewGuid();
            var stopwatch = Stopwatch.StartNew();
            var parent = CurrentOperation.Value;
            var operation = new Operation(operationId, stopwatch, context, parent);
            CurrentOperation.Value = operation;
            return operation;
        }

        public Guid OperationId { get; }
        public Stopwatch Stopwatch { get; }
        public Context Context { get; }
        public Operation Parent { get; }

        public OperationSnapshot TakeSnapshot()
        {
            return new OperationSnapshot(OperationId, Stopwatch.Elapsed, Context, Parent.TakeSnapshot());
        }

        public static OperationSnapshot SnapshotCurrentOperation()
        {
            return CurrentOperation.Value.TakeSnapshot();
        }
    }
}
