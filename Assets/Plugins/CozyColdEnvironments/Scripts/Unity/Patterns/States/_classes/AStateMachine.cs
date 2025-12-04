#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.TypeMatching;
using CCEnvs.Unity.Components;

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

        public bool IsPlaying() => State.IsSome;
        public bool IsPlaying<T>()
            where T : IState
        {
            return State.Map(x => x.Is<T>()).GetValue(false);
        }

        protected void SetState(IState? state)
        {
            State = state.Maybe();
        }
    }
}
