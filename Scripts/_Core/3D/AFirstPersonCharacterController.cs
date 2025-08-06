using UnityEngine;

#nullable enable
#pragma warning disable IDE0044
#pragma warning disable S2933
namespace UTIRLib
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class AFirstPersonCharacterController : WorldObject
    {
        protected IMoveStrategy moveStrategy = null!;
        protected IRotationStrategy cameraRotationStrategy = null!;
        protected IRotationStrategy bodyRotationStrategy = null!;

        [GetBySelf]
        protected Rigidbody rb = null!;

        public float MoveSpeed {
            get => moveStrategy.MoveSpeed;
            set => moveStrategy.SetMoveSpeed(value);
        }

        public float CameraSensivity {
            get => cameraRotationStrategy.RotationSpeed;
            set => cameraRotationStrategy.SetRotationSpeed(value);
        }
    }
}
