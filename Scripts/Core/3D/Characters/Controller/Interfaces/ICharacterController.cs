#nullable enable
using UnityEngine;

namespace UTIRLib.ThreeD.Characters
{
    public interface ICharacterController
    {
        void Move(Vector3 direction, float speed);
    }
}
