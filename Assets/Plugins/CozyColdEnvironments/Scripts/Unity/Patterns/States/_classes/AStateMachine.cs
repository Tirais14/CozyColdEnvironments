#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CCEnvs.TypeMatching;
using CCEnvs.Unity.Components;
using System;
using System.Collections.Generic;

namespace CCEnvs.Patterns.States
{
    public abstract class AStateMachine : CCBehaviour
    {
        protected Maybe<IState> State { get; private set; } = null!;

        protected virtual void Update()
        {
            if (State.TryGetValue(out IState? state))
                state.Tick();
        }

        protected virtual void FixedUpdate()
        {
            if (State.TryGetValue(out IState? state))
                state.FixedTick();
        }

        protected virtual void LateUpdate()
        {
            if (State.TryGetValue(out IState? state))
                state.LateTick();
        }

        public bool IsPlaying(Type? stateType)
        {
            if (stateType.IsNotType<IState>())
                return false;

            return (State.IsNone && stateType is null)
                    ||
                    (State.TryGetValue(out var state) && state.GetType() == stateType);
        }

        public bool IsPlaying() => State.IsSome;
        public bool IsPlaying<T>()
            where T : IState
        {
            return IsPlaying(typeof(T));
        }

        public bool IsPlaying(IState? state)
        {
            return IsPlaying(state?.GetType());
        }

        public bool IsPlaying<T>(T? state)
            where T : IState
        {
            return IsPlaying((IState?)state);
        }

        protected void SetState(IState? state)
        {
            if (EqualityComparer<IState?>.Default.Equals(state, State.Raw))
                return;

            State.IfSome(x => x.Exit());

            State = state.Maybe();
            State.IfSome(x => x.Enter());
        }
    }
}
