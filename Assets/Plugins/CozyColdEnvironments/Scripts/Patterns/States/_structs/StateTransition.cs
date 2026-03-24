using CommunityToolkit.Diagnostics;

#nullable enable
namespace CCEnvs.Patterns.States
{
    public class StateTransition : IStateTransition
    {
        public static StateTransitionBuilder Builder => new();

        public IState NextState { get; }

        public IStateTransitionPredicate Condition { get; }

        public StateTransition(
            IState nextState,
            IStateTransitionPredicate predicate
            )
        {
            CC.Guard.IsNotNull(nextState, nameof(nextState));
            Guard.IsNotNull(predicate, nameof(predicate));

            NextState = nextState;
            Condition = predicate;
        }
    }
}
