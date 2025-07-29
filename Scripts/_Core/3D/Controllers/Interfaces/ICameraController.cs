#nullable enable
using UnityEngine;

namespace UTIRLib.Controllers
{
    public interface ICameraController
    {
        Camera Camera { get; }
        IRotationController RotationController { get; }
        float RotationSpeed { get; }

        void SetRotationSpeed(float newRotationSpeed);
    }
}
