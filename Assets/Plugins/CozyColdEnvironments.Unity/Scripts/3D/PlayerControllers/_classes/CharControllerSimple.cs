using CCEnvs.Dependencies;
using CCEnvs.Disposables;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.EditorSerialization;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.InputSystem.Rx;
using R3;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3.Controllers
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public class CharControllerSimple : CCBehaviour
    {
        public const float SURFACE_CAST_DISTANCE_MIN = 0.01f;
        public const float MOVE_SPEED_MIN = 0f;
        public const float AIR_MOVE_SPEED_MIN = 0f;
        public const float JUMP_HEIGHT_MIN = 0f;
        public const float FLY_MOVE_SPEED_MIN = 0f;

        public const float SURFACE_CAST_DISTANCE_DEFAULT = 0.4f;
        public const float GRAVITY_DEFAULT = -35f;

        [Header("Settings")]
        [Space(6f)]

        [SerializeField, Min(MOVE_SPEED_MIN)]
        protected float moveSpeed = 6f;

        [SerializeField, Min(FLY_MOVE_SPEED_MIN)]
        protected float airSpeedModifier = 0.5f;

        [SerializeField, Min(JUMP_HEIGHT_MIN)]
        protected float jumpHeight = 2.7f;

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
        private bool hasMoveIA;

        private IDisposable? jumpIABinding;

        [field: GetBySelf]
        public CharacterController core { get; private set; } = null!;

        public float MoveSpeed => moveSpeed;
        public float AirSpeedModifier => airSpeedModifier;
        public float JumpHeight => jumpHeight;
        public float SurfaceCastDistance => surfaceCastDistance;
        public float Gravity => gravity;

        public Transform SurfaceCastPoint => surfaceCastPoint;

        public ButtonActionRx? JumpIA { get; private set; }

        public InputActionRx<Vector2>? MoveIA { get; private set; }

        public LayerMask? SurfaceLayers => surfaceLayers;

        public bool IsGrounded { get; private set; }

        protected override void Start()
        {
            base.Start();

            TryResolveInputActions();
            TryBindJumpInputAction();
        }

        protected virtual void Update()
        {
            if (hasMoveIA && MoveIA!.Action.IsPressed())
                MoveByInput(MoveIA.Action.ReadValue<Vector2>());
        }

        protected virtual void LateUpdate()
        {
            RaycastSurface();
            HandlePhysics();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CCDisposable.Dispose(ref jumpIABinding);
        }

        public CharControllerSimple SetMoveSpeed(float value)
        {
            moveSpeed = Mathf.Max(MOVE_SPEED_MIN, value);

            return this;
        }

        public CharControllerSimple SetAirSpeedModifier(float value)
        {
            airSpeedModifier = Mathf.Max(AIR_MOVE_SPEED_MIN, value);

            return this;
        }

        public CharControllerSimple SetJumpHeight(float value)
        {
            jumpHeight = Mathf.Max(JUMP_HEIGHT_MIN, value);

            return this;
        }

        public CharControllerSimple SetSurfaceCastPoint(Transform value)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return this;
        }

        public CharControllerSimple SetGravity(float value)
        {
            gravity = value;

            return this;
        }

        public CharControllerSimple SetSurfaceLayers(LayerMask? value)
        {
            surfaceLayers = value;

            return this;
        }

        public CharControllerSimple SetSurfaceCastDistance(float value)
        {
            surfaceCastDistance = Mathf.Max(SURFACE_CAST_DISTANCE_MIN, value);

            return this;
        }

        public CharControllerSimple SetMoveInputAction(InputActionRx<Vector2>? value)
        {
            MoveIA = value;

            hasMoveIA = value != null;

            return this;
        }

        public CharControllerSimple SetMoveInputAction(ButtonActionRx? value)
        {
            JumpIA = value;

            TryBindJumpInputAction();

            return this;
        }

        public void Move(Vector3 dir)
        {
            moveDirection = dir;
        }

        public void MoveByInput(Vector2 inputValue)
        {
            var dir = new Vector3(inputValue.x, 0f, inputValue.y).normalized;

            Move(dir);
        }

        public void Jump()
        {
            if (jumpRequested || !IsGrounded)
                return;

            jumpRequested = true;
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

        private void HandlePhysics()
        {
            if (IsGrounded && velocity.y < 0f)
                velocity.y = -2f;

            var motion = transform.right * moveDirection.x + transform.forward * moveDirection.z;

            core.Move(moveSpeed * Time.deltaTime * motion);

            if (jumpRequested)
                TryJump();

            velocity.y += gravity * Time.deltaTime;

            core.Move(velocity * Time.deltaTime);

            moveDirection = default;
        }

        private void TryResolveInputActions()
        {
            MoveIA = CCServices.TryResolve<InputActionRx<Vector2>>(CCServices.MOVE_INPUT_ACTION_CONTAINER_KEY);
            JumpIA = CCServices.TryResolve<ButtonActionRx>(CCServices.JUMP_INPUT_ACTION_CONTAINER_KEY);

            hasMoveIA = MoveIA != null;
        }

        private void TryBindJumpInputAction()
        {
            CCDisposable.Dispose(ref jumpIABinding);

            if (JumpIA is not null)
            {
                jumpIABinding = JumpIA.ObservePerformed()
                    .Subscribe(this,
                    static (_, @this) =>
                    {
                        @this.Jump();
                    });
            }
        }
    }
}
