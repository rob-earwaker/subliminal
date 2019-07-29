using System;
using System.Reactive.Linq;

namespace Subliminal
{
    public class DerivedTrigger<TContext> : ITrigger<TContext>
    {
        private DerivedTrigger(Guid triggerId, IObservable<ActivatedTrigger<TContext>> activated)
        {
            TriggerId = triggerId;
            Activated = activated;
        }

        public static DerivedTrigger<TContext> FromObservable(IObservable<TContext> context)
        {
            var triggerId = Guid.NewGuid();

            var activated = context
                .Take(1)
                .Timestamp()
                .Select(ctx => new ActivatedTrigger<TContext>(triggerId, ctx.Value, ctx.Timestamp));

            return new DerivedTrigger<TContext>(triggerId, activated);
        }

        public Guid TriggerId { get; }
        public IObservable<ActivatedTrigger<TContext>> Activated { get; }
    }
}
