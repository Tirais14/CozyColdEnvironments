#nullable enable
using CommunityToolkit.Diagnostics;
using System.Collections.Generic;

namespace CCEnvs.Patterns.States
{
    public class CompositeState : IState
    {
        private readonly List<IState> states = new();

        public string ID { get; }

        public CompositeState(string id, params IState[] states)
        {
            Guard.IsNotNull(id, nameof(id));
            Guard.IsNotNull(states, nameof(states));

            ID = id;
            AddRange(states);
        }

        public CompositeState Add(IState state)
        {
            CC.Guard.IsNotNull(state, nameof(state));

            states.Add(state);
            return this;
        }

        public CompositeState AddRange(IEnumerable<IState> states)
        {
            CC.Guard.IsNotNull(states, nameof(states));

            this.states.AddRange(states);
            return this;
        }

        public bool Remove(IState state)
        {
            CC.Guard.IsNotNull(state, nameof(state));

            return states.Remove(state);
        }
    }
}
