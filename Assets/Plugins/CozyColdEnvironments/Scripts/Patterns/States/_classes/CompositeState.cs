#nullable enable
using CommunityToolkit.Diagnostics;
using System;
using System.Linq;

namespace CCEnvs.Patterns.States
{
    public abstract class CompositeState : IState
    {
        private readonly IState[] states;

        public abstract Type StateType { get; }

        public CompositeState(params IState[] states)
        {
            Guard.IsNotNull(states, nameof(states));

            this.states = states.ToArray();
        }

        //public CompositeState Add(IState state)
        //{
        //    CC.Guard.IsNotNull(state, nameof(state));

        //    states.Add(state);
        //    return this;
        //}

        //public CompositeState AddRange(IEnumerable<IState> states)
        //{
        //    CC.Guard.IsNotNull(states, nameof(states));

        //    this.states.AddRange(states);
        //    return this;
        //}

        //public bool Remove(IState state)
        //{
        //    CC.Guard.IsNotNull(state, nameof(state));

        //    return states.Remove(state);
        //}
    }
}
