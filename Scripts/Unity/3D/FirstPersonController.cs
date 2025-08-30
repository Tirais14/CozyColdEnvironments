using CCEnvs.Common;
using CCEnvs.Unity.ComponentSetter;
using CCEnvs.Unity.Extensions;
using CCEnvs.Unity.Linq;
using UniRx;
using UnityEngine;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Unity.ThreeD
{
    [RequireComponent(typeof(Rigidbody))]
    public class FirstPersonController : GameModel
    {
        private const int MAX_RAYCAST_HITS = 3;
        private const float RAYCAST_SURFACE_OFFSET = 10f;

        private readonly ReactiveProperty<bool> isMovingReactive = new();
        private readonly ReactiveProperty<bool> isOnSurfaceReactive = new();
        private readonly ReactiveProperty<bool> isJumpingReactive = new();

        private float moveSpeed = 1000f;
        private float jumpForce = 12f;
        private RaycastHit surfaceHit;

        [GetBySelf]
        protected Rigidbody rigidBody { get; private set; } = null!;

        [GetByChildren]
        [field: SerializeField]
        protected Camera characterCamera { get; private set; } = null!;

        protected Transform characterCameraTransform { get; private set; } = null!;

        public bool IsMoving {
            get => isMovingReactive.Value;
            protected set => isMovingReactive.Value = value;
        }

        public IReadOnlyReactiveProperty<bool> IsMovingReactive => isMovingReactive;

        public bool IsOnSurface {
            get => isOnSurfaceReactive.Value;
            protected set => isOnSurfaceReactive.Value = value;
        }

        public IReadOnlyReactiveProperty<bool> IsOnSurfaceReactive => isOnSurfaceReactive;

        public bool IsJumping {
            get => isJumpingReactive.Value;
            protected set => isJumpingReactive.Value = value;
        }

        public IReadOnlyReactiveProperty<bool> IsJumpingReactive => isJumpingReactive;

        public float MoveSpeed {
            get => moveSpeed;
            set
            {
                if (value < 0f)
                    value = 0f;

                moveSpeed = value;
            }
        }
        public float JumpForce {
            get => jumpForce;
            set
            {
                if (value < 0f)
                    value = 0f;

                jumpForce = value;
            }
        }
        [field: SerializeField]
        public LayerMask SurfaceMask { get; set; }

        protected override void OnStart()
        {
            base.OnStart();

            characterCameraTransform = characterCamera.transform;
            BindReactiveProps();
        }

        private void Update()
        {
            IsOnSufaceObserver();
        }

        private void FixedUpdate()
        {
            if (IsMoving)
                SnapToSurface();
        }

        public void Jump(bool inputValue)
        {
            if (!inputValue || !IsOnSurface)
                return;

            IsJumping = true;
            rigidBody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        }

        public void Move(Direction2D direction2D)
        {
            if (!IsOnSurface)
                return;

            IsMoving = true;
            Vector3 direction = GetMoveDirection(direction2D);

            Vector3 velocity = Time.fixedDeltaTime * MoveSpeed * direction;
            velocity.y = rigidBody.linearVelocity.y;

            rigidBody.linearVelocity = velocity;
        }
        public void Move(Vector2 rawInputValue)
        {
            Move(rawInputValue.ToDirection2D());
        }

        private static Quaternion GetRotationForMoveDirection(Direction2D direction2D)
        {
            return direction2D switch
            {
                Direction2D.None => default,
                Direction2D.Down => Quaternion.Euler(0f, 180f, 0f),
                Direction2D.Left => Quaternion.Euler(0f, -90, 0f),
                Direction2D.Right => Quaternion.Euler(0f, 90, 0f),
                Direction2D.Up => Quaternion.Euler(0f, 0f, 0f),
                Direction2D.LeftDown => Quaternion.Euler(0f, -135, 0f),
                Direction2D.LeftUp => Quaternion.Euler(0f, -45, 0f),
                Direction2D.RightDown => Quaternion.Euler(0f, 135, 0f),
                Direction2D.RightUp => Quaternion.Euler(0f, 45, 0f),
                _ => throw new System.InvalidOperationException(direction2D.ToString()),
            };
        }

        private void BindReactiveProps()
        {
            isOnSurfaceReactive.Subscribe(x =>
            {
                if (!x)
                    return;

                IsJumping = false;

            }).AddTo(this);
        }

        private Vector3 GetMoveDirection(Direction2D direction2D)
        {
            if (direction2D == Direction2D.None)
                return Vector3.zero;

            Quaternion vectorRotation = GetRotationForMoveDirection(direction2D);

            return (vectorRotation * characterCameraTransform.forward).normalized;
        }

        private void IsOnSufaceObserver()
        {
            var raycastOrigin = transform.position.Q()
                                                  .SetY(transform.position.y + RAYCAST_SURFACE_OFFSET);

            if (!Physics.Raycast(raycastOrigin,
                                 Vector3.down,
                                 out RaycastHit hit,
                                 Mathf.Infinity,
                                 SurfaceMask,
                                 QueryTriggerInteraction.Ignore))
            {
                CCDebug.PrintWarning("Not found any surface.", this);
                return;
            }

            CCDebug.AssertError(hit.transform != transform, "Collide with self.");

            IsOnSurface = hit.distance.NearlyEquals(RAYCAST_SURFACE_OFFSET, 1f);
            surfaceHit = hit;
        }

        private void SnapToSurface()
        {
            float lerpedY = Mathf.Lerp(rigidBody.position.y, surfaceHit.point.y, 0.01f);
            rigidBody.MovePosition(rigidBody.position.Q().SetY(lerpedY));
        }
    }
}
