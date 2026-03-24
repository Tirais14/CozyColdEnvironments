using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Patterns.States
{
    public class AnonymousStateTransitionPredicate : IStateTransitionPredicate
    {
        private readonly Func<bool> predicate;

        public AnonymousStateTransitionPredicate(Func<bool> predicate)
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            this.predicate = predicate;
        }

        public bool Evaluate() => predicate();
    }

    public class AnonymousStateTransitionPredicate<TArg> : IStateTransitionPredicate
    {
        private readonly TArg arg;
        private readonly Func<TArg, bool> predicate;

        public AnonymousStateTransitionPredicate(
            TArg arg,
            Func<TArg, bool> predicate
            )
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            this.arg = arg;
            this.predicate = predicate;
        }

        public bool Evaluate() => predicate(arg);
    }
}
