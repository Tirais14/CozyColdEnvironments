using UnityEngine;

#nullable enable
namespace CCEnvs.Patterns.States
{
    public interface IStateTransition
    {
        IState NextState { get; }

        IStateTransitionPredicate Condition { get; }
    }
}
