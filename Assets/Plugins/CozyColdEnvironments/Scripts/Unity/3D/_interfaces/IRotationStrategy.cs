#nullable enable
namespace CCEnvs.Unity._3D
{
    public interface IRotationStrategy
    {
        float RotationSpeed { get; }

        void SetRotationSpeed(float newRotationSpeed);

        void Rotate(float deltaTime);
    }
}
