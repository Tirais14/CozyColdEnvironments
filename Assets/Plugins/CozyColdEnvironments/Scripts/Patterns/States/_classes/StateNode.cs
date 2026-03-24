using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Patterns.States
{
    public class StateNode : IStateNode
    {
        private readonly List<IStateTransition> transitions = new();

        public IState State { get; }

        public IReadOnlyList<IStateTransition> Transitions => transitions;

        public StateNode(IState state)
        {
            CC.Guard.IsNotNull(state, nameof(state));

            State = state;
        }

        public StateNode(IState state, IEnumerable<IStateTransition> transitions)
            :
            this(state)
        {
            CC.Guard.IsNotNull(transitions, nameof(transitions));

            this.transitions.AddRange(transitions);
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
