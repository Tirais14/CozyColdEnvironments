#nullable enable
using UnityEngine;

namespace UTIRLib.Controllers
{
    public interface IMovementController
    {
        void Move(Vector3 direction, float speed);
    }
}
