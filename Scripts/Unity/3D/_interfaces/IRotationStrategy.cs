#nullable enable
namespace CCEnvs.Unity.ThreeD
{
    public interface IRotationStrategy
    {
        float RotationSpeed { get; }

        void SetRotationSpeed(float newRotationSpeed);

        void Rotate(float deltaTime);
    }
}
