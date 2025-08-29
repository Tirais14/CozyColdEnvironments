#nullable enable
namespace CozyColdEnvironments
{
    public interface IMoveStrategy 
    {
        float MoveSpeed { get; }

        void SetMoveSpeed(float newMoveSpeed);

        void Move(float deltaTime);
    }
}
