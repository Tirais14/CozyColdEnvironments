#nullable enable
namespace CCEnvs.Unity.ThreeD
{
    public interface IMoveStrategy 
    {
        float MoveSpeed { get; }

        void SetMoveSpeed(float newMoveSpeed);

        void Move(float deltaTime);
    }
}
