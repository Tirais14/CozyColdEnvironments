using System;
using UTIRLib.Patterns.State;

#nullable enable

namespace UTIRLib.Patterns.StateMachine
{
    public readonly struct StateTransition
    {
        private readonly Func<bool> completePredicate;
        public readonly Type? nextStateType;
        public readonly bool canAbortPrevious;

        public bool IsCompleted => completePredicate();

        public StateTransition(Func<bool> completePredicate,
                               Type? nextStateType,
                               bool canAbortPrevious)
        {
            this.completePredicate = completePredicate ?? throw new ArgumentNullException(nameof(completePredicate));
            this.nextStateType = nextStateType;
            this.canAbortPrevious = canAbortPrevious;
        }

        public StateTransition(Func<bool> completePredicate,
                               Type? nextStateType) : this(completePredicate,
                                                           nextStateType,
                                                           false)
        {
        }

        public StateTransition(Func<bool> completePredicate,
                               bool canAbortPrevious) : this(completePredicate,
                                                             nextStateType: null,
                                                             canAbortPrevious)
        {
        }

        public StateTransition(Func<bool> completePredicate) : this(completePredicate,
                                                                    nextStateType: null,
                                                                    false)
        {
        }

        public static StateTransition Create<TNextState>(Func<bool> completePredicate, bool canAbortPrevious = false)
            where TNextState : IReadOnlyState
        {
            return new StateTransition(completePredicate, typeof(TNextState), canAbortPrevious);
        }
    }
}