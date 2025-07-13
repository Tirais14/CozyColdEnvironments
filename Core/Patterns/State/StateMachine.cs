#nullable enable
using System;
using UTIRLib.Diagnostics;

namespace UTIRLib.Patterns.States
{
    public class StateMachine<T> : IStateMachine<T>
        where T : class, IStateBase
    {
        private readonly IStateMachineSwitchStrategy<T> switchStrategy;
        private readonly T defaultState;
        private T previousState;

        protected T playingState;

        public Type PlayingStateType => playingState.GetType();

        public StateMachine(IStateMachineSwitchStrategy<T> switchStrategy)
        {
            defaultState = switchStrategy.DefaultState;

            this.switchStrategy = switchStrategy;
            playingState = defaultState;

            previousState = defaultState;
        }

        public void Execute()
        {
            playingState = switchStrategy.GetNextState();
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void PlayState(T state)
        {
            if (state.IsNull())
                throw new ArgumentNullException(nameof(state));

            if (playingState == state)
                return;

            previousState = playingState;

            playingState.Exit();
            playingState = state;
            playingState.Enter();
        }

        public void PlayPreviousState() => PlayState(previousState);

        public void PlayDefaultState() => PlayState(defaultState);

        public abstract class SwitchStrategy : IStateMachineSwitchStrategy<T>
        {
            public abstract T DefaultState { get; }

            public abstract T GetNextState();
        }
    }
}
