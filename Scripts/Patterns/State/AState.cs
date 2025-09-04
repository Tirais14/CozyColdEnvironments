#nullable enable
using CCEnvs.Common;
using CCEnvs.Reflection;

namespace CCEnvs.Patterns.States
{
    public abstract class AState<T> : IState
        where T : IStateMachine
    {
        protected readonly T stateMachine;

        protected AState(T stateMachine) => this.stateMachine = stateMachine;

        protected virtual void OnEnter()
        {
        }

        protected virtual void OnTick()
        {
        }

        protected virtual void OnFixedTick()
        {
        }

        protected virtual void OnLateTick()
        {
        }

        protected virtual void OnExit()
        {
        }

        protected void ForceStopState(string? message = null!)
        {
            stateMachine.ForceStopState(this);

            if (message.IsNotNullOrEmpty())
                CCDebug.PrintWarning($"{this.GetTypeName()} stopped. {message.Trim('.')}.");
        }

        void IState.Enter() => OnEnter();

        void IState.Tick() => OnTick();

        void IState.FixedTick() => OnFixedTick();

        void IState.LateTick() => OnLateTick();

        void IState.Exit() => OnExit();
    }
}
