#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using UTIRLib.Diagnostics;

namespace UTIRLib.Patterns.States
{
    public class StateMachine<T> : IStateMachine<T>
        where T : class, IStateBase
    {
        private readonly IStateMachineSwitchStrategy<T> switchStrategy;

        public T DefaultState { get; }
        public T PreviousState { get; private set; }
        public T PlayingState { get; private set; }

        public Type PlayingStateType => PlayingState.GetType();

        public StateMachine(IStateMachineSwitchStrategy<T> switchStrategy)
        {
            DefaultState = switchStrategy.DefaultState;

            this.switchStrategy = switchStrategy;
            PlayingState = DefaultState;

            PreviousState = DefaultState;
        }

        public void Execute()
        {
            PlayingState = switchStrategy.GetNextState();
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void PlayState(T state)
        {
            if (state.IsNull())
                throw new ArgumentNullException(nameof(state));

            if (PlayingState == state)
                return;

            PreviousState = PlayingState;

            PlayingState.Exit();
            PlayingState = state;
            PlayingState.Enter();
        }

        public void PlayPreviousState() => PlayState(PreviousState);

        public void PlayDefaultState() => PlayState(DefaultState);

        public abstract class SwitchStrategy : IStateMachineSwitchStrategy<T>
        {
            [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
            protected Type selectedStateType => SelectedState.GetType();

            public T DefaultState { get; } = null!;
            public T PreviousState { get; protected set; } = null!;
            public T SelectedState { get; protected set; } = null!;

            protected SwitchStrategy(T defaultState)
            {
                DefaultState = defaultState;
                PreviousState = defaultState;
                SelectedState = defaultState;
            }

            public abstract T GetNextState();

            /// <exception cref="ArgumentNullException"></exception>
            public void ForceSelectState(T state)
            {
                if (state is null)
                    throw new ArgumentNullException(nameof(state));

                SelectedState = state;
            }

            public void ResetState() => SelectedState = DefaultState;
        }
    }
}
