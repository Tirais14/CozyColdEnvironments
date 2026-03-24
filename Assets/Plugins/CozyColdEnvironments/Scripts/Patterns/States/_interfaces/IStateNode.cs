using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Patterns.States
{
    public interface IStateNode
    {
        IState State { get; }

        IReadOnlyList<IStateTransition> Transitions { get; }

        IStateNode AddTransition(IStateTransition transition);

        IStateNode RemoveTransition(IStateTransition transition);
    }
}
