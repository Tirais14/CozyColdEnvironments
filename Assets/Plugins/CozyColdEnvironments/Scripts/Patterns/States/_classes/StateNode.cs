using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Patterns.States
{
    public class StateNode : IStateNode
    {
        private readonly List<IStateTransition> transitions = new();

        public IState State { get; }

        public IReadOnlyCollection<IStateTransition> Transitions => transitions;

        public StateNode(IState state)
        {
            CC.Guard.IsNotNull(state, nameof(state));

            State = state;
        }

        public void AddTransition(IStateTransition transition)
        {
            CC.Guard.IsNotNull(transition, nameof(transition));

            transitions.Add(transition);
        }

        public void RemoveTransition(IStateTransition transition)
        {
            CC.Guard.IsNotNull(transition, nameof(transition));

            transitions.Remove(transition);
        }
    }
}
