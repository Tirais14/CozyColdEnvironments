#nullable enable
namespace CCEnvs.Unity._3D
{
    public interface IMoveStrategy 
    {
        float MoveSpeed { get; }

        void SetMoveSpeed(float newMoveSpeed);

        void Move(float deltaTime);
    }
}
