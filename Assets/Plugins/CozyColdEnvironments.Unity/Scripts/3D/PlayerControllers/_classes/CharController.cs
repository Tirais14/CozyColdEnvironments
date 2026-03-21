using CCEnvs.Unity.Components;
using CCEnvs.Unity.EditorSerialization;
using CCEnvs.Unity.Injections;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
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
        protected float moveSpeed = 6f;

        [SerializeField, Min(FLY_MOVE_SPEED_MIN)]
        protected float airSpeedModifier = 0.5f;

        [SerializeField, Min(JUMP_HEIGHT_MIN)]
        protected float jumpHeight = 1.6f;

        [SerializeField, Min(RUN_SPEED_MODIFIER_MIN)]
        protected float runSpeedModifier = 1.45f;

        [Header("Physics")]
        [Space(6f)]

        [SerializeField]
        protected Transform surfaceCastPoint;

        [SerializeField]
        protected SerializedNullable<LayerMask> surfaceLayers;

        [SerializeField, Min(SURFACE_CAST_DISTANCE_MIN)]
        protected float surfaceCastDistance = SURFACE_CAST_DISTANCE_DEFAULT;

        [SerializeField]
        protected float gravity = GRAVITY_DEFAULT;

        private Vector3 velocity;
        private Vector3 moveDirection;

        private bool jumpRequested;

        public float MoveSpeed => moveSpeed;
        public float AirSpeedModifier => airSpeedModifier;
        public float JumpHeight => jumpHeight;
        public float RunSpeedModifier => runSpeedModifier;
        public float SurfaceCastDistance => surfaceCastDistance;
        public float Gravity => gravity;

        [field: GetBySelf]
        public CharacterController core { get; private set; } = null!;

        public Transform SurfaceCastPoint => surfaceCastPoint;

        public LayerMask? SurfaceLayers => surfaceLayers;

        public bool IsGrounded { get; private set; }
        public bool IsRunning { get; private set; }

        protected override void Start()
        {
            base.Start();
        }

        protected virtual void Update()
        {
            // Movement is now handled by CharacterControllerInputBinder via MoveByInput
        }

        protected virtual void LateUpdate()
        {
            RaycastSurface();
            HandlePhysics();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public CharController SetMoveSpeed(float value)
        {
            moveSpeed = Mathf.Max(MOVE_SPEED_MIN, value);
            return this;
        }

        public CharController SetAirSpeedModifier(float value)
        {
            airSpeedModifier = Mathf.Max(AIR_MOVE_SPEED_MIN, value);
            return this;
        }

        public CharController SetJumpHeight(float value)
        {
            jumpHeight = Mathf.Max(JUMP_HEIGHT_MIN, value);
            return this;
        }

        public CharController SetRunSpeedModifier(float value)
        {
            runSpeedModifier = Mathf.Max(value, RUN_SPEED_MODIFIER_MIN);
            return this;
        }

        public CharController SetSurfaceCastPoint(Transform value)
        {
            CC.Guard.IsNotNull(value, nameof(value));
            surfaceCastPoint = value;
            return this;
        }

        public CharController SetGravity(float value)
        {
            gravity = value;
            return this;
        }

        public CharController SetSurfaceLayers(LayerMask? value)
        {
            surfaceLayers = value;
            return this;
        }

        public CharController SetSurfaceCastDistance(float value)
        {
            surfaceCastDistance = Mathf.Max(SURFACE_CAST_DISTANCE_MIN, value);
            return this;
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

        private void RaycastSurface()
        {
            IsGrounded = Physics.CheckSphere(
                surfaceCastPoint.transform.position,
                surfaceCastDistance / 2,
                surfaceLayers.Deserialized ?? Physics.AllLayers
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

            var motion = transform.right * moveDirection.x + transform.forward * moveDirection.z;
            var moveSpeed = ResolveMoveSpeed();

            core.Move(moveSpeed * Time.deltaTime * motion);

            if (jumpRequested)
                TryJump();

            velocity.y += gravity * Time.deltaTime;

            core.Move(velocity * Time.deltaTime);

            moveDirection = default;
            IsRunning = false;
        }
    }
}