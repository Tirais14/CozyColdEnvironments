#nullable enable
using R3;
using System;

namespace CCEnvs.Patterns.States
{
    public interface IStateMachine
    {
        Type? CurrentStateType { get; }

        void Tick();

        void FixedTick();

        void LateTick();

        IStateTransition? ResolveTransition();

        void SetState(Type? stateType);
        void SetState<T>();

        IStateMachine AddNode(IStateNode node);

        bool RemoveNode(Type stateType);

        bool ContainsNode(Type? stateType);

        IStateMachine AddTransition(IStateTransition transition);

        bool RemoveTransition(IStateTransition transition);

        bool ContainsTransition(IStateTransition? transition);

        Observable<Type?> ObserveCurrentStateType();
    }
}
