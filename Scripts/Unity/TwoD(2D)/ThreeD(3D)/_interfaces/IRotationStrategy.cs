#nullable enable
namespace UTIRLib
{
    public interface IRotationStrategy
    {
        float RotationSpeed { get; }

        void SetRotationSpeed(float newRotationSpeed);

        void Rotate(float deltaTime);
    }
}
