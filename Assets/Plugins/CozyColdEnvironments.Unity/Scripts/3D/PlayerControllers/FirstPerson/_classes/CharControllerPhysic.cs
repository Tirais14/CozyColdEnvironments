using CCEnvs.Diagnostics;
using CCEnvs.Patterns.Commands;
using CCEnvs.Unity.Async;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.EditorSerialization;
using CCEnvs.Unity.Injections;
using Cysharp.Threading.Tasks;
using R3;
using System.IO.Pipelines;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3.PlayerControllers
{
    public abstract class CharControllerPhysic : CCBehaviour
    {
        public const float SURFACE_CAST_DISTANCE_MIN = 1f;
        public const float MOVE_SPEED_MIN = 0f;
        public const float JUMP_FORCE_MIN = 0f;
        public const float FLY_MOVE_SPEED_MIN = 0f;
        public const float FLY_THRESHOLD_MIN = 0.06f;

        [SerializeField, GetBySelf]
        protected Rigidbody _rigidBody = null!;

        [SerializeField]
        protected SerializedNullable<LayerMask> surfaceCastLayerMask;

        [SerializeField, Min(SURFACE_CAST_DISTANCE_MIN)]
        protected float surfaceCastDistance = 100f;

        [SerializeField, Min(MOVE_SPEED_MIN)]
        protected float moveSpeed = 3f;

        [SerializeField, Min(FLY_MOVE_SPEED_MIN)]
        protected float flyMoveSpeed = 1.2f;

        [SerializeField, Min(JUMP_FORCE_MIN)]
        protected float jumpForce = 5.5f;

        [SerializeField, Min(FLY_THRESHOLD_MIN)]
        protected float flyThreshold = FLY_THRESHOLD_MIN;

        [SerializeField]
        protected bool canJumpInFly;

        [SerializeField]
        protected float skinWidth = 0.05f;

        //protected readonly CommandScheduler commandScheduler = new(UnityFrameProvider.Update, nameof(CharControllerPhysic));

        protected RaycastHit? surfaceCastHit;

        protected Vector3 lastSurfaceCastPos;

        private ReactiveCommand<CharControllerPhysic>? onGrounded;

        private long? jumpFrame;

        private long? moveFrame;

        private Vector3? onJumpVelocity;

        public new Rigidbody rigidbody => _rigidBody;

        public new CapsuleCollider collider { get; private set; } = null!;

        public RaycastHit? SurfaceCastHit => surfaceCastHit;

        public LayerMask? SurfaceCastLayerMask {
            get => surfaceCastLayerMask;
            set => surfaceCastLayerMask = value;
        }

        public float SurfaceCastDistance {
            get => surfaceCastDistance;
            set => surfaceCastDistance = Mathf.Max(SURFACE_CAST_DISTANCE_MIN, value);
        }

        public float MoveSpeed {
            get => moveSpeed;
            set => moveSpeed = Mathf.Max(MOVE_SPEED_MIN, value);
        }

        public float FlyMoveSpeed {
            get => flyMoveSpeed;
            set => Mathf.Max(FLY_MOVE_SPEED_MIN, value);
        }

        public float JumpForce {
            get => jumpForce;
            set => jumpForce = Mathf.Max(JUMP_FORCE_MIN, value);
        }

        public float FlyThreshold {
            get => flyThreshold;
            set => flyThreshold = Mathf.Max(FLY_THRESHOLD_MIN, value);
        }

        public bool CanJumpInFly {
            get => canJumpInFly;
            set => canJumpInFly = value;
        }

        public bool IsMoving { get; private set; }

        public bool IsJumping { get; private set; }

        protected override void Start()
        {
            base.Start();
            collider = rigidbody.Q().Component<CapsuleCollider>().Strict();
        }

        protected virtual void Update()
        {
        }

        protected virtual void FixedUpdate()
        {
            if (IsGrounded())
            {
                rigidbody.useGravity = false;

                if (IsMoving)
                {
                    StopMoving();
                    SnapToGround();
                }

                OnGrounded();
            }
            else
                rigidbody.useGravity = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            onGrounded?.Dispose();
        }

        public CharControllerPhysic StopMoving()
        {
            rigidbody.linearVelocity = rigidbody.linearVelocity.WithX(0f).WithZ(0f);

            IsMoving = false;

            return this;
        }

        public CharControllerPhysic StopJumping()
        {
            rigidbody.linearVelocity = rigidbody.linearVelocity.WithY(0f);

            //IsJumping = false;

            return this;
        }

        public void Move(
            Vector3 dir
            )
        {
            if (dir == Vector3.zero)
            {
                StopMoving();
                return;
            }

            if (IsGrounded())
            {
                RaycastSurfaceIfExpired();

                dir = Vector3.ProjectOnPlane(dir, surfaceCastHit!.Value.normal);

                rigidbody.linearVelocity = (dir * moveSpeed).WithY(rigidbody.linearVelocity.y);
            }
            else
                rigidbody.linearVelocity = (dir * flyMoveSpeed).WithY(rigidbody.linearVelocity.y);

            lastSurfaceCastPos = rigidbody.position;

            IsMoving = true;
        }

        public void MoveByInput(Vector2 inputValue)
        {
            var dir = new Vector3(inputValue.x, 0f, inputValue.y);

            Move(dir);
        }

        public void Jump(Vector3? dir = null)
        {
            if (IsJumping)
                return;

            if (jumpForce.NearlyEquals(0f))
                return;

            if (!canJumpInFly && !IsGrounded())
                return;

            dir ??= rigidbody.transform.up.normalized;

            var directedForce = jumpForce * dir.Value;

            if (directedForce == Vector3.zero)
                return;

            rigidbody.linearVelocity = rigidbody.linearVelocity.WithY(0f);
            rigidbody.AddForce(directedForce, ForceMode.Impulse);
        }

        public float GetDistanceFromGround()
        {
            RaycastSurfaceIfExpired();

            if (!surfaceCastHit.HasValue)
                return surfaceCastDistance;

            return surfaceCastHit.Value.distance;
        }

        public bool IsGrounded()
        {
            return GetDistanceFromGround() < flyThreshold;
        }

        public Observable<CharControllerPhysic> ObserveOnGrounded()
        {
            onGrounded ??= new ReactiveCommand<CharControllerPhysic>();

            return onGrounded;
        }

        private void RaycastSurface()
        {
            //var rayOrigin = collider.bounds.min + Vector3.up * 0.05f;

            //if (Physics.Raycast(
            //     rayOrigin,
            //     -rigidbody.transform.up,
            //     out var hit,
            //     surfaceCastDistance,
            //     surfaceCastLayerMask.Deserialized ?? Physics.AllLayers,
            //     QueryTriggerInteraction.Ignore
            //     ))
            //{
            //     surfaceCastHit = hit;

            //    if (CCDebug.Instance.IsEnabled && hit.transform == rigidbody.transform)
            //        this.PrintError("Self raycasting");
            //}
            //else
            //    surfaceCastHit = null;


            var colliderCenter = collider.transform.position;

            float radius = collider.radius;
            float height = collider.height;

            float spehereOffset = (height / 2) - radius;

            var point1 = colliderCenter + Vector3.up * spehereOffset;
            var point2 = colliderCenter - Vector3.down * spehereOffset;

            var startPoint1 = point1 + Vector3.up * skinWidth;
            var startPoint2 = point2 + Vector3.up * skinWidth;

            if (Physics.CapsuleCast(
                startPoint1,
                startPoint2,
                radius,
                Vector3.down,
                out var hit,
                surfaceCastDistance,
                surfaceCastLayerMask.Deserialized ?? Physics.AllLayers,
                QueryTriggerInteraction.Ignore
                ))
            {
                surfaceCastHit = hit;

                if (CCDebug.Instance.IsEnabled && hit.transform == rigidbody.transform)
                    this.PrintError("Self raycasting");
            }
            else
                surfaceCastHit = null;

            lastSurfaceCastPos = rigidbody.position;
        }

        private void RaycastSurfaceIfExpired()
        {
            if (IsSurfaceCastExpired())
                RaycastSurface();
        }

        private bool IsSurfaceCastExpired()
        {
            if (!surfaceCastHit.HasValue)
                return true;

            var epsilon = 0.07f;

            var pos = rigidbody.position;

            return lastSurfaceCastPos.x.NotNearlyEquals(pos.x, epsilon)
                   ||
                   lastSurfaceCastPos.y.NotNearlyEquals(pos.y, epsilon)
                   ||
                   lastSurfaceCastPos.z.NotNearlyEquals(pos.z, epsilon);
        }

        private void OnGrounded()
        {
            StopJumping();
            rigidbody.useGravity = false;
            onGrounded?.Execute(this);
        }

        private void SnapToGround()
        {
            RaycastSurfaceIfExpired();

            if (!surfaceCastHit.HasValue)
                return;

            var pos = rigidbody.position;

            rigidbody.MovePosition(Vector3.Lerp(rigidbody.position, new Vector3(pos.x, surfaceCastHit.Value.point.y, pos.z), 0.015f));
            //rigidbody.AddForce(new Vector3(0f, -230f), ForceMode.Impulse);
        }
    }
}
