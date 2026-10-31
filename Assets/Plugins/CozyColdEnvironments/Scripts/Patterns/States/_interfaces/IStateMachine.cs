#nullable enable
using R3;
using System;

namespace CCEnvs.Patterns.States
{
    public interface IStateMachine
    {
        string? CurrentStateID { get; }

        void Tick();

        void FixedTick();

        void LateTick();

        IStateTransition? ResolveTransition();

        void SetState(Type? stateType);
        void SetState<T>();
        void SetState(string id);

        IStateMachine AddNode(IStateNode node);

        bool RemoveNode(Type? stateType);
        bool RemoveNode<T>();
        bool RemoveNode(string? id);

        bool ContainsNode(Type? stateType);
        bool ContainsNode<T>();
        bool ContainsNode(string id);

        IStateMachine AddTransition(IStateTransition transition);

        bool RemoveTransition(IStateTransition transition);

        bool ContainsTransition(IStateTransition? transition);

        Observable<string?> ObserveCurrentStateID();
    }
}
