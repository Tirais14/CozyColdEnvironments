using CommunityToolkit.Diagnostics;

#nullable enable
namespace CCEnvs.Patterns.States
{
    public partial class StateTransition
    {
        public static IStateTransition Empty => Builder.WithNextState(null)
            .WithPredicate(StateTransitionPredicate.True)
            .Build();

        public static StateTransitionBuilder Builder => new();
    }

    public partial class StateTransition : IStateTransition
    {
        public IState? NextState { get; }

        public IStateTransitionPredicate Condition { get; }

        public StateTransition(
            IState? nextState,
            IStateTransitionPredicate predicate
            )
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            NextState = nextState;
            Condition = predicate;
        }
    }
}
