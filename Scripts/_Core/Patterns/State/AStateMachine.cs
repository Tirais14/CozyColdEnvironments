#nullable enable
using System;
using UTIRLib.Diagnostics;

namespace UTIRLib.Patterns.States
{
    public abstract class AStateMachine : MonoX
    {
        protected IState defaultState;
        protected IState playingState;

        protected override void OnStart()
        {
            base.OnStart();

            playingState = defaultState;
            playingState.Enter();
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

        protected abstract IState GetNextState();

        private void Update()
        {
            SetState(GetNextState());
            playingState.OnUpdate();
        }

        private void FixedUpdate() => playingState.OnFixedUpdate();

        private void LateUpdate() => playingState.OnLateUpdate();
    }
}
