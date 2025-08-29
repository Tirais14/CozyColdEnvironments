#nullable enable
namespace CozyColdEnvironments
{
    public interface IRotationStrategy
    {
        float RotationSpeed { get; }

        void SetRotationSpeed(float newRotationSpeed);

        void Rotate(float deltaTime);
    }
}
