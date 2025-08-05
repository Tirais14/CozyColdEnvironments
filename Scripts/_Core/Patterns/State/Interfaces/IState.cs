#nullable enable
namespace UTIRLib.Patterns.States
{
    public interface IState
    {
        void Enter();

        void OnUpdate();

        void OnFixedUpdate();

        void OnLateUpdate();

        void Exit();
    }
}
