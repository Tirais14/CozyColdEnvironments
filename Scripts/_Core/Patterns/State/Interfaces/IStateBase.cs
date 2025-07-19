#nullable enable
namespace UTIRLib.Patterns.States
{
    public interface IStateBase
    {
        void Enter();

        void Exit();

        bool CanTransitionTo(IStateBase stateBase);
    }
}
