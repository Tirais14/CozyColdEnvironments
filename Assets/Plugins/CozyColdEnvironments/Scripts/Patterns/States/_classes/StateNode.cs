using CommunityToolkit.Diagnostics;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Patterns.States
{
    public class StateNode : IStateNode
    {
        private readonly List<IStateTransition> transitions = new();

        public IState State { get; }

        public IReadOnlyList<IStateTransition> Transitions => transitions;

        public StateNode(IState? state)
        {
            State = state;
        }

        public StateNode(IState state, params IStateTransition[] transitions)
            :
            this(state)
        {
            Guard.IsNotNull(state, nameof(state));

            this.transitions.AddRange(transitions);
        }

        public StateNode(IState state, IEnumerable<IStateTransition> transitions)
            :
            this(state)
        {
            CC.Guard.IsNotNull(transitions, nameof(transitions));

            this.transitions.AddRange(transitions);
        }

        public IStateNode AddTransition(IStateTransition transition)
        {
            CC.Guard.IsNotNull(transition, nameof(transition));

            transitions.Add(transition);
            return this;
        }

        public IStateNode RemoveTransition(IStateTransition transition)
        {
            CC.Guard.IsNotNull(transition, nameof(transition));

            transitions.Remove(transition);
            return this;
        }

        public override string ToString()
        {
            return $"{nameof(State)}: {State}";
        }
    }
}
