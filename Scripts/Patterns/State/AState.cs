#nullable enable
namespace UTIRLib.Patterns.States
{
    public abstract class AState<T> : IState
        where T : AStateMachine
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

        protected void StopExecuting() => stateMachine.ForceStopState(this);

        void IState.Enter() => OnEnter();

        void IState.Tick() => OnTick();

        void IState.FixedTick() => OnFixedTick();

        void IState.LateTick() => OnLateTick();

        void IState.Exit() => OnExit();
    }
}
