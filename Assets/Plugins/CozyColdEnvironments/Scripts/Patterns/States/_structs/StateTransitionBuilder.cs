using CCEnvs.Collections;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Patterns.States
{
    public struct StateTransitionBuilder
    {
        private List<IStateTransitionPredicate>? predicates;
        private IState? nextState;

        public StateTransitionBuilder WithNextState(IState? nextState)
        {
            this.nextState = nextState;
            return this;
        }

        public StateTransitionBuilder WithPredicate(IStateTransitionPredicate predicate)
        {
            CC.Guard.IsNotNull(predicate, nameof(predicate));

            predicates ??= new List<IStateTransitionPredicate>();
            predicates.Add(predicate);
            return this;
        }

        public StateTransitionBuilder WithPredicate(Func<bool> predicateFunc)
        {
            Guard.IsNotNull(predicateFunc, nameof(predicateFunc));

            var predicate = new AnonymousStateTransitionPredicate(predicateFunc);
            WithPredicate(predicate);
            return this;
        }

        public StateTransitionBuilder WithPredicate<TArg>(TArg arg, Func<TArg, bool> predicateFunc)
        {
            Guard.IsNotNull(predicateFunc, nameof(predicateFunc));

            var predicate = new AnonymousStateTransitionPredicate<TArg>(arg, predicateFunc);
            WithPredicate(predicate);
            return this;
        }

        public StateTransitionBuilder WithPredicate<TArg1, TArg2>(TArg1 arg1, TArg2 arg2, Func<TArg1, TArg2, bool> predicateFunc)
        {
            Guard.IsNotNull(predicateFunc, nameof(predicateFunc));

            var predicate = new AnonymousStateTransitionPredicate<TArg1, TArg2>(arg1, arg2, predicateFunc);
            WithPredicate(predicate);
            return this;
        }

        public StateTransitionBuilder WithPredicate<TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3, Func<TArg1, TArg2, TArg3, bool> predicateFunc)
        {
            Guard.IsNotNull(predicateFunc, nameof(predicateFunc));

            var predicate = new AnonymousStateTransitionPredicate<TArg1, TArg2, TArg3>(arg1, arg2, arg3, predicateFunc);
            WithPredicate(predicate);
            return this;
        }

        public StateTransitionBuilder WithPredicate<TArg1, TArg2, TArg3, TArg4>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, Func<TArg1, TArg2, TArg3, TArg4, bool> predicateFunc)
        {
            Guard.IsNotNull(predicateFunc, nameof(predicateFunc));

            var predicate = new AnonymousStateTransitionPredicate<TArg1, TArg2, TArg3, TArg4>(arg1, arg2, arg3, arg4, predicateFunc);
            WithPredicate(predicate);
            return this;
        }

        public StateTransitionBuilder WithPredicate<TArg1, TArg2, TArg3, TArg4, TArg5>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, Func<TArg1, TArg2, TArg3, TArg4, TArg5, bool> predicateFunc)
        {
            Guard.IsNotNull(predicateFunc, nameof(predicateFunc));

            var predicate = new AnonymousStateTransitionPredicate<TArg1, TArg2, TArg3, TArg4, TArg5>(arg1, arg2, arg3, arg4, arg5, predicateFunc);
            WithPredicate(predicate);
            return this;
        }

        public StateTransitionBuilder WithPredicate<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, bool> predicateFunc)
        {
            Guard.IsNotNull(predicateFunc, nameof(predicateFunc));

            var predicate = new AnonymousStateTransitionPredicate<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(arg1, arg2, arg3, arg4, arg5, arg6, predicateFunc);
            WithPredicate(predicate);
            return this;
        }

        public readonly IStateTransition Build()
        {
            IStateTransitionPredicate predicate;

            if (predicates.IsNullOrEmpty())
                predicate = StateTransitionPredicate.True;
            else if (predicates.Count == 1)
                predicate = predicates[0];
            else
                predicate = new CompositeStateTransitionPredicate(predicates);

            return new StateTransition(nextState, predicate);
        }

        public StateTransitionBuilder Reset()
        {
            predicates?.Clear();
            nextState = null;

            return this;
        }
    }
}
