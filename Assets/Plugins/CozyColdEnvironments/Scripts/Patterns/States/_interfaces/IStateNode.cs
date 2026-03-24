using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Patterns.States
{
    public interface IStateNode
    {
        IState State { get; }

        IReadOnlyCollection<IStateTransition> Transitions { get; }

        void AddTransition(IStateTransition transition);

        void RemoveTransition(IStateTransition transition);
    }
}
