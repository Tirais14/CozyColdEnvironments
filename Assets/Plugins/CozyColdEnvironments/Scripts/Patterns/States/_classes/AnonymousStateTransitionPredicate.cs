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

    public class AnonymousStateTransitionPredicate<TArg1, TArg2> : IStateTransitionPredicate
    {
        private readonly TArg1 arg1;
        private readonly TArg2 arg2;
        private readonly Func<TArg1, TArg2, bool> predicate;

        public AnonymousStateTransitionPredicate(
            TArg1 arg1,
            TArg2 arg2,
            Func<TArg1, TArg2, bool> predicate
            )
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            this.arg1 = arg1;
            this.arg2 = arg2;
            this.predicate = predicate;
        }

        public bool Evaluate() => predicate(arg1, arg2);
    }

    public class AnonymousStateTransitionPredicate<TArg1, TArg2, TArg3> : IStateTransitionPredicate
    {
        private readonly TArg1 arg1;
        private readonly TArg2 arg2;
        private readonly TArg3 arg3;
        private readonly Func<TArg1, TArg2, TArg3, bool> predicate;

        public AnonymousStateTransitionPredicate(
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            Func<TArg1, TArg2, TArg3, bool> predicate
            )
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            this.arg1 = arg1;
            this.arg2 = arg2;
            this.arg3 = arg3;
            this.predicate = predicate;
        }

        public bool Evaluate() => predicate(arg1, arg2, arg3);
    }

    public class AnonymousStateTransitionPredicate<TArg1, TArg2, TArg3, TArg4> : IStateTransitionPredicate
    {
        private readonly TArg1 arg1;
        private readonly TArg2 arg2;
        private readonly TArg3 arg3;
        private readonly TArg4 arg4;
        private readonly Func<TArg1, TArg2, TArg3, TArg4, bool> predicate;

        public AnonymousStateTransitionPredicate(
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            TArg4 arg4,
            Func<TArg1, TArg2, TArg3, TArg4, bool> predicate
            )
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            this.arg1 = arg1;
            this.arg2 = arg2;
            this.arg3 = arg3;
            this.arg4 = arg4;
            this.predicate = predicate;
        }

        public bool Evaluate() => predicate(arg1, arg2, arg3, arg4);
    }

    public class AnonymousStateTransitionPredicate<TArg1, TArg2, TArg3, TArg4, TArg5> : IStateTransitionPredicate
    {
        private readonly TArg1 arg1;
        private readonly TArg2 arg2;
        private readonly TArg3 arg3;
        private readonly TArg4 arg4;
        private readonly TArg5 arg5;
        private readonly Func<TArg1, TArg2, TArg3, TArg4, TArg5, bool> predicate;

        public AnonymousStateTransitionPredicate(
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            TArg4 arg4,
            TArg5 arg5,
            Func<TArg1, TArg2, TArg3, TArg4, TArg5, bool> predicate
            )
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            this.arg1 = arg1;
            this.arg2 = arg2;
            this.arg3 = arg3;
            this.arg4 = arg4;
            this.arg5 = arg5;
            this.predicate = predicate;
        }

        public bool Evaluate() => predicate(arg1, arg2, arg3, arg4, arg5);
    }

    public class AnonymousStateTransitionPredicate<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> : IStateTransitionPredicate
    {
        private readonly TArg1 arg1;
        private readonly TArg2 arg2;
        private readonly TArg3 arg3;
        private readonly TArg4 arg4;
        private readonly TArg5 arg5;
        private readonly TArg6 arg6;
        private readonly Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, bool> predicate;

        public AnonymousStateTransitionPredicate(
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            TArg4 arg4,
            TArg5 arg5,
            TArg6 arg6,
            Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, bool> predicate
            )
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            this.arg1 = arg1;
            this.arg2 = arg2;
            this.arg3 = arg3;
            this.arg4 = arg4;
            this.arg5 = arg5;
            this.arg6 = arg6;
            this.predicate = predicate;
        }

        public bool Evaluate() => predicate(arg1, arg2, arg3, arg4, arg5, arg6);
    }
}
