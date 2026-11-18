using CCEnvs.UnityX.Components;
using CCEnvs.UnityX.EditorSerialization;
using CCEnvs.UnityX.ComponentInjections;
using R3;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.D3
{
    public class CharController : CCBehaviour
    {
        public const float SURFACE_CAST_DISTANCE_MIN = 0.01f;
        public const float MOVE_SPEED_MIN = 0f;
        public const float AIR_MOVE_SPEED_MIN = 0f;
        public const float JUMP_HEIGHT_MIN = 0f;
        public const float FLY_MOVE_SPEED_MIN = 0f;
        public const float RUN_SPEED_MODIFIER_MIN = 0f;

        public const float SURFACE_CAST_DISTANCE_DEFAULT = 0.4f;
        public const float GRAVITY_DEFAULT = -35f;

        [Header("Settings")]
        [Space(6f)]

        [SerializeField, Min(MOVE_SPEED_MIN)]
        private float moveSpeed = 6f;
        public float MoveSpeed
        {
            get => moveSpeed;
            private set => moveSpeed = Mathf.Max(MOVE_SPEED_MIN, value);
        }

        [SerializeField, Min(FLY_MOVE_SPEED_MIN)]
        private float airSpeedModifier = 0.5f;
        public float AirSpeedModifier
        {
            get => airSpeedModifier;
            private set => airSpeedModifier = Mathf.Max(AIR_MOVE_SPEED_MIN, value);
        }

        [SerializeField, Min(JUMP_HEIGHT_MIN)]
        private float jumpHeight = 1.6f;
        public float JumpHeight
        {
            get => jumpHeight;
            private set => jumpHeight = Mathf.Max(JUMP_HEIGHT_MIN, value);
        }

        [SerializeField, Min(RUN_SPEED_MODIFIER_MIN)]
        private float runSpeedModifier = 1.45f;
        public float RunSpeedModifier
        {
            get => runSpeedModifier;
            private set => runSpeedModifier = Mathf.Max(RUN_SPEED_MODIFIER_MIN, value);
        }

        [Header("Physics")]
        [Space(6f)]

        [SerializeField]
        private Transform surfaceCastPoint;
        public Transform SurfaceCastPoint
        {
            get => surfaceCastPoint;
            private set => surfaceCastPoint = value;
        }

        [SerializeField]
        private SerializedNullable<LayerMask> surfaceLayers;
        public LayerMask? SurfaceLayers
        {
            get => surfaceLayers;
            private set => surfaceLayers = value;
        }

        [SerializeField, Min(SURFACE_CAST_DISTANCE_MIN)]
        private float surfaceCastDistance = SURFACE_CAST_DISTANCE_DEFAULT;
        public float SurfaceCastDistance
        {
            get => surfaceCastDistance;
            private set => surfaceCastDistance = Mathf.Max(SURFACE_CAST_DISTANCE_MIN, value);
        }

        [SerializeField]
        private float gravity = GRAVITY_DEFAULT;
        public float Gravity
        {
            get => gravity;
            private set => gravity = value;
        }

        private readonly ReactiveProperty<Vector3> lastTravelDistance = new();

        private Vector3 velocity;
        private Vector3 moveDirection;

        private bool jumpRequested;

        [field: GetBySelf]
        public CharacterController core { get; private set; } = null!;

        public bool IsGrounded { get; private set; }
        public bool IsRunning { get; private set; }

        protected virtual void Update()
        {
            // Movement is now handled by CharacterControllerInputBinder via MoveByInput
        }

        protected virtual void LateUpdate()
        {
            if (core.enabled)
            {
                RaycastSurface();
                HandlePhysics();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            lastTravelDistance.Dispose();
        }

        public CharController Run(bool state = true)
        {
            if (state && IsGrounded)
                IsRunning = true;
            else
                IsRunning = false;

            return this;
        }

        public CharController Move(Vector3 dir)
        {
            moveDirection = dir;
            return this;
        }

        public CharController MoveByInput(Vector2 inputValue)
        {
            var dir = new Vector3(inputValue.x, 0f, inputValue.y).normalized;

            return Move(dir);
        }

        public CharController Jump()
        {
            if (!jumpRequested && IsGrounded)
                jumpRequested = true;

            return this;
        }

        public Observable<Vector3> ObserveTravelDistance() => lastTravelDistance;

        private void RaycastSurface()
        {
            IsGrounded = Physics.CheckSphere(
                surfaceCastPoint.transform.position,
                surfaceCastDistance / 2,
                surfaceLayers.Data ?? Physics.AllLayers
                );
        }

        private void TryJump()
        {
            if (!IsGrounded)
                return;

            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpRequested = false;
        }

        private float ResolveMoveSpeed()
        {
            if (IsRunning)
                return moveSpeed * RunSpeedModifier;
            else if (!IsGrounded)
                return moveSpeed * airSpeedModifier;

            return moveSpeed;
        }

        private void HandlePhysics()
        {
            if (IsGrounded && velocity.y < 0f)
                velocity.y = -2f;

            if (moveDirection != Vector3.zero)
                ApplyMoveDirection();

            if (jumpRequested)
                TryJump();

            ApplyGravity();

            moveDirection = default;
            IsRunning = false;
        }

        private void ApplyMoveDirection()
        {
            var dir = cTransform.right * moveDirection.x + cTransform.forward * moveDirection.z;
            var moveSpeed = ResolveMoveSpeed();
            var motion = moveSpeed * Time.deltaTime * dir;
            var preivousPos = cTransform.position;

            core.Move(motion);

            var travelDistance = preivousPos - cTransform.position;
            lastTravelDistance.Value = travelDistance;
        }

        private void ApplyGravity()
        {
            velocity.y += gravity * Time.deltaTime;

            core.Move(velocity * Time.deltaTime);
        }
    }
}
