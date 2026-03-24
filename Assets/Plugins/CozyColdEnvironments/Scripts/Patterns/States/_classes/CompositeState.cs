using CommunityToolkit.Diagnostics;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Patterns.States
{
    public class CompositeState : IState
    {
        private readonly List<IState> states = new();

        public string ID { get; }

        public CompositeState(string id)
        {
            Guard.IsNotNull(id, nameof(id));

            ID = id;
        }

        public CompositeState(string id, IEnumerable<IState> states)
            :
            this(id)
        {
            CC.Guard.IsNotNull(states, nameof(states));

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
