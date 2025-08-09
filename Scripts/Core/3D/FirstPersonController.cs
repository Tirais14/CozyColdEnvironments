using UnityEngine;
using UTIRLib.Unity.Extensions;

#nullable enable
#pragma warning disable IDE1006
namespace UTIRLib
{
    [RequireComponent(typeof(Rigidbody))]
    public class FirstPersonController : WorldObject
    {
        private float moveSpeed = 1000f;

        new protected Transform transform { get; private set; } = null!;

        [GetBySelf]
        protected Rigidbody rigidBody { get; private set; } = null!;

        [GetByChildren]
        [field: SerializeField]
        protected Camera characterCamera { get; private set; } = null!;

        protected Transform cameraTransform { get; private set; } = null!;

        public float MoveSpeed {
            get => moveSpeed;
            set
            {
                if (value < 0f)
                    value = 0f;

                moveSpeed = value;
            }
        }

        private Vector3 GetMoveDirection(Vector2 rawInputValue)
        {
            if (rawInputValue == Vector2.zero)
                return rawInputValue;

            Direction2D direction = rawInputValue.ToDirection2D();

            switch (direction)
            {
                case Direction2D.None:
                    return Vector3.zero;
                case Direction2D.Down:
                    return Quaternion.Euler(0f, 180f, 0f) * cameraTransform.forward;
                case Direction2D.Left:
                    return Quaternion.Euler(0f, -90, 0f) * cameraTransform.forward;
                case Direction2D.Right:
                    return Quaternion.Euler(0f, 90, 0f) * cameraTransform.forward;
                case Direction2D.Up:
                    return cameraTransform.forward;
                case Direction2D.LeftDown:
                    return Quaternion.Euler(0f, -135, 0f) * cameraTransform.forward;
                case Direction2D.LeftUp:
                    return Quaternion.Euler(0f, -45, 0f) * cameraTransform.forward;
                case Direction2D.RightDown:
                    return Quaternion.Euler(0f, 135, 0f) * cameraTransform.forward;
                case Direction2D.RightUp:
                    return Quaternion.Euler(0f, 45, 0f) * cameraTransform.forward;
                default:
                    throw new System.InvalidOperationException(direction.ToString());
            }
        }

        public void Move(Vector2 rawInputValue)
        {
            Vector3 direction = GetMoveDirection(rawInputValue);

            RigidBodyHelper.MoveByPhysics(rigidBody, direction, moveSpeed);
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            transform = base.transform;
        }

        protected override void OnStart()
        {
            base.OnStart();

            cameraTransform = characterCamera.transform;
        }
    }
}
