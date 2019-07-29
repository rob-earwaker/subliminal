using System;

namespace Subliminal
{
    public class ActivatedTrigger<TContext>
    {
        public ActivatedTrigger(Guid triggerId, TContext context, DateTimeOffset timestamp)
        {
            TriggerId = triggerId;
            Context = context;
            Timestamp = timestamp;
        }

        public Guid TriggerId { get; }
        public TContext Context { get; }
        public DateTimeOffset Timestamp { get; }
    }
}
