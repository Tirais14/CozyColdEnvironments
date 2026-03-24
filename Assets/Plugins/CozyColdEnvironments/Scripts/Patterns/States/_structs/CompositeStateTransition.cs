using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Patterns.States
{
    public class CompositeStateTransition : IStateTransition
    {
        private readonly IStateTransition[] transitions = Array.Empty<IStateTransition>();

        public CompositeStateTransition(IStateTransition[] transitions)
        {
            Guard.IsNotNull(transitions, nameof(transitions));

            this.transitions = transitions;
        }

        public bool CanTransit()
        {
            for (int i = 0; i < transitions.Length; i++)
            {
                if (!transitions[i].CanTransit())
                    return false;
            }

            return true;
        }
    }
}
