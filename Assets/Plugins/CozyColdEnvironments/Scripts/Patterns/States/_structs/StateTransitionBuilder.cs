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

        public StateTransitionBuilder WithNextState(IState nextState)
        {
            CC.Guard.IsNotNull(nextState, nameof(nextState));

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

        public readonly IStateTransition Build()
        {
            if (nextState.IsNull())
                throw new InvalidOperationException("Cannot create state transition without next state");

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
