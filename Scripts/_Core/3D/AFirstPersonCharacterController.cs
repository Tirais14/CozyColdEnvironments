using UnityEngine;

#nullable enable
#pragma warning disable IDE0044
#pragma warning disable S2933
namespace UTIRLib
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class AFirstPersonCharacterController : MonoX
    {
        [GetBySelf]
        protected Rigidbody rb = null!;
        protected IMoveStrategy moveStrategy = null!;
        protected IRotationStrategy cameraRotationStrategy = null!;

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
