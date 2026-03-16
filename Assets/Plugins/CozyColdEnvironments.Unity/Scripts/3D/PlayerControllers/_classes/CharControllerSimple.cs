using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3.Controllers
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public class CharControllerSimple : CCBehaviour
    {
        public const float SURFACE_CAST_DISTANCE_MIN = 1f;
        public const float MOVE_SPEED_MIN = 0f;
        public const float JUMP_FORCE_MIN = 0f;
        public const float FLY_MOVE_SPEED_MIN = 0f;
        public const float FLY_THRESHOLD_MIN = 0.06f;

        [SerializeField, Min(MOVE_SPEED_MIN)]
        protected float moveSpeed = 3f;

        [SerializeField, Min(FLY_MOVE_SPEED_MIN)]
        protected float flyMoveSpeed = 1.2f;

        [SerializeField, Min(JUMP_FORCE_MIN)]
        protected float jumpHeight = 5.5f;

        [SerializeField, Min(0f)]
        protected float gravity = 9.81f;

        private Vector3 velocity;
        private Vector3 moveDirection;

        [field: GetBySelf]
        public CharacterController controllerBase { get; private set; } = null!;

        public float MoveSpeed {
            get => moveSpeed;
            set => moveSpeed = Mathf.Max(MOVE_SPEED_MIN, value);
        }

        public float FlyMoveSpeed {
            get => flyMoveSpeed;
            set => Mathf.Max(FLY_MOVE_SPEED_MIN, value);
        }

        public float JumpForce {
            get => jumpHeight;
            set => jumpHeight = Mathf.Max(JUMP_FORCE_MIN, value);
        }

        public bool IsGrounded => controllerBase.isGrounded;

        protected virtual void Update()
        {
            HandlePhysics();
        }

        public CollisionFlags Move(Vector3 dir)
        {
            if (dir == Vector3.zero)
                return default;

            var motion = ApplySpeedToDirection(dir);

            return controllerBase.Move(motion);
        }

        public void MoveByInput(Vector2 inputValue)
        {
            var dir = new Vector3(inputValue.x, 0f, inputValue.y).normalized;

            Move(dir);
        }

        public CollisionFlags Jump()
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            return controllerBase.Move(velocity);
        }

        private Vector3 ApplySpeedToDirection(Vector3 dir)
        {
            if (controllerBase.isGrounded)
                return moveSpeed * Time.deltaTime * dir;

            return flyMoveSpeed * Time.deltaTime * dir;

        }

        private void ApplyGravity()
        {
            controllerBase.Move(Physics.gravity * Time.deltaTime);
        }

        private void SnapToGround()
        {
            velocity.y = -0.5f;
        }

        private void HandlePhysics()
        {
            if (IsGrounded && velocity.y < 0)
                SnapToGround();

            velocity.y += gravity * Time.deltaTime;
        }
    }
}
