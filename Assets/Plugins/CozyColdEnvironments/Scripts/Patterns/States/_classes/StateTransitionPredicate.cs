#nullable enable
using System;

namespace CCEnvs.Patterns.States
{
    public static class StateTransitionPredicate
    {
        public static IStateTransitionPredicate True { get; } = new AnonymousStateTransitionPredicate(() => true);
        public static IStateTransitionPredicate False { get; } = new AnonymousStateTransitionPredicate(() => false);

        public static IStateTransitionPredicate Inverse(this IStateTransitionPredicate predicate)
        {
            CC.Guard.IsNotNull(predicate, nameof(predicate));

            return Create(predicate, static predicate => !predicate.Evaluate());
        }

        public static IStateTransitionPredicate Create(Func<bool> predicateFunc)
        {
            return new AnonymousStateTransitionPredicate(predicateFunc);
        }

        public static IStateTransitionPredicate Create<TArg>(TArg arg, Func<TArg, bool> predicateFunc)
        {
            return new AnonymousStateTransitionPredicate<TArg>(arg, predicateFunc);
        }

        public static IStateTransitionPredicate Create<TArg1, TArg2>(TArg1 arg1, TArg2 arg2, Func<TArg1, TArg2, bool> predicateFunc)
        {
            return new AnonymousStateTransitionPredicate<TArg1, TArg2>(arg1, arg2, predicateFunc);
        }

        public static IStateTransitionPredicate Create<TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3, Func<TArg1, TArg2, TArg3, bool> predicateFunc)
        {
            return new AnonymousStateTransitionPredicate<TArg1, TArg2, TArg3>(arg1, arg2, arg3, predicateFunc);
        }

        public static IStateTransitionPredicate Create<TArg1, TArg2, TArg3, TArg4>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, Func<TArg1, TArg2, TArg3, TArg4, bool> predicateFunc)
        {
            return new AnonymousStateTransitionPredicate<TArg1, TArg2, TArg3, TArg4>(arg1, arg2, arg3, arg4, predicateFunc);
        }

        public static IStateTransitionPredicate Create<TArg1, TArg2, TArg3, TArg4, TArg5>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, Func<TArg1, TArg2, TArg3, TArg4, TArg5, bool> predicateFunc)
        {
            return new AnonymousStateTransitionPredicate<TArg1, TArg2, TArg3, TArg4, TArg5>(arg1, arg2, arg3, arg4, arg5, predicateFunc);
        }

        public static IStateTransitionPredicate Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, bool> predicateFunc)
        {
            return new AnonymousStateTransitionPredicate<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(arg1, arg2, arg3, arg4, arg5, arg6, predicateFunc);
        }
    }
}
