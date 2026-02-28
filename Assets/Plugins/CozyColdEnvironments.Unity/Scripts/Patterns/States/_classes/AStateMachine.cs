#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CCEnvs.Unity.Components;
using System;
using System.Collections.Generic;

namespace CCEnvs.Patterns.States
{
    public abstract class AStateMachine : CCBehaviour
    {
        private IState? idleState = null!;

        public bool IsIdle { get; private set; }

        protected Maybe<IState> State { get; private set; } = null!;

        protected override void Start()
        {
            base.Start();

            idleState = CreateIdleState();

            SetIdle();
        }

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

        protected override void OnDisable()
        {
            base.OnDisable();
            SetIdle();
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

            if (State.TryGetValue(out var prevState))
                prevState.Exit();

            State = state.Maybe();

            if (state.IsNotNull())
            {
                state.Enter();

                if (state.Equals(idleState))
                    IsIdle = true;
            }
        }

        protected void SetIdle()
        {
            SetState(idleState);
        }

        protected abstract IState? CreateIdleState();
    }
}
