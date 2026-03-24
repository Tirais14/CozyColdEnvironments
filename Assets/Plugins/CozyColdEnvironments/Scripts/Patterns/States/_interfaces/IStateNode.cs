using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Patterns.States
{
    public interface IStateNode
    {
        IState State { get; }

        IReadOnlyList<IStateTransition> Transitions { get; }

        void AddTransition(IStateTransition transition);

        void RemoveTransition(IStateTransition transition);
    }
}
