using UnityEngine;

#nullable enable
namespace UTIRLib.Controllers
{
    public interface IRotationController
    {
        void Rotate(Vector3 angles, float speed);
    }
}
