using UnityEngine;

#pragma warning disable IDE0044
#nullable enable
namespace UTIRLib.ThreeD.Characters
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterControllerX : MonoX, ICharacterController
    {
        [GetBySelf]
        private Rigidbody rb = null!;

        public void Move(Vector3 direction, float speed)
        {
            Vector3 target = transform.position + (direction.normalized * speed);

            rb.MovePosition(target);
        }
        public void Move(Vector2 direction, float speed)
        {
            Move(new Vector3(direction.x, 0, direction.y), speed);
        }
    }
}
