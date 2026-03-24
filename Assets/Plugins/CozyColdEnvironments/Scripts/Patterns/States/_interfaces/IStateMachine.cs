#nullable enable
using System.Collections.Generic;

namespace CCEnvs.Patterns.States
{
    public interface IStateMachine
    {
        void Tick();

        void FixedTick();

        void LateTick();

        IStateTransition? ResolveTransition();

        void SetState(string id);

        IStateNode GetOrCreateNode(
            IState state,
            IEnumerable<IStateTransition>? transitions = null
            );

        void AddNode(IStateNode node);

        bool RemoveNode(string id);

        bool ContainsNode(string? id);

        void AddTransition(IStateTransition transition);

        bool RemoveTransition(IStateTransition transition);

        bool ContainsTransition(IStateTransition? transition);
    }
}
