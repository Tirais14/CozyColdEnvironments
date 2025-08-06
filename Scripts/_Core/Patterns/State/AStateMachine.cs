#nullable enable
using System;
using UTIRLib.Diagnostics;
using UTIRLib.Patterns.Factory;

namespace UTIRLib.Patterns.States
{
    public abstract class AStateMachine : MonoX
    {
        protected IState defaultState;
        protected IState playingState;

        public bool IsDefaultState => IsPlaying(defaultState);

        protected override void OnStart()
        {
            base.OnStart();

            playingState = defaultState;
            playingState.Enter();
        }

        public abstract IState GetNextState();

        public bool IsPlaying(IState state) => playingState.Equals(state);

        protected void CreateStatesByFactory(IFactory<Type, IState>? factory = null)
        {
            StateMachineHelper.CreateStatesByFactory(this, factory);
        }

        /// <exception cref="ArgumentNullException"></exception>
        protected void SetState(IState state)
        {
            if (playingState == state)
                return;

            playingState.Exit();
            playingState = state;
            playingState.Enter();
        }

        private void Update()
        {
            SetState(GetNextState());

            playingState.OnUpdate();
        }

        private void FixedUpdate() => playingState.OnFixedUpdate();

        private void LateUpdate() => playingState.OnLateUpdate();
    }
}
