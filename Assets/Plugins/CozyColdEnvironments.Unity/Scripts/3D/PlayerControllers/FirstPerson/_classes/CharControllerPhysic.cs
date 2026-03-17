#nullable enable
namespace CCEnvs.Unity.D3.PlayerControllers
{
    //public abstract class CharControllerPhysic : CCBehaviour
    //{
    //    public const float SURFACE_CAST_DISTANCE_MIN = 1f;
    //    public const float MOVE_SPEED_MIN = 0f;
    //    public const float JUMP_FORCE_MIN = 0f;
    //    public const float FLY_MOVE_SPEED_MIN = 0f;
    //    public const float FLY_THRESHOLD_MIN = 0.06f;

    //    [SerializeField, GetBySelf]
    //    protected Rigidbody _rigidBody = null!;

    //    [SerializeField]
    //    protected SerializedNullable<LayerMask> surfaceCastLayerMask;

    //    [SerializeField]
    //    protected float acceleration = 10f;

    //    [SerializeField]
    //    protected float deceleration = 15f;

    //    [SerializeField, Min(SURFACE_CAST_DISTANCE_MIN)]
    //    protected float surfaceCastDistance = 100f;

    //    [SerializeField, Min(MOVE_SPEED_MIN)]
    //    protected float moveSpeed = 3f;

    //    [SerializeField, Min(FLY_MOVE_SPEED_MIN)]
    //    protected float airMoveModifier = 1.2f;

    //    [SerializeField, Min(JUMP_FORCE_MIN)]
    //    protected float jumpForce = 5.5f;

    //    [SerializeField]
    //    protected float maxSlopeAngle = 45f;

    //    [SerializeField]
    //    protected float coyoteTime = 0.1f;

    //    private bool isGrounded;
    //    private bool jumpRequested;

    //    private Vector3 moveDirection;

    //    private float coyoteTimeCounter;
    //    private float originalLinearDampging;

    //    public new Rigidbody rigidbody => _rigidBody;

    //    public new CapsuleCollider collider { get; private set; } = null!;

    //    public LayerMask? SurfaceCastLayerMask {
    //        get => surfaceCastLayerMask;
    //        set => surfaceCastLayerMask = value;
    //    }

    //    public float SurfaceCastDistance {
    //        get => surfaceCastDistance;
    //        set => surfaceCastDistance = Mathf.Max(SURFACE_CAST_DISTANCE_MIN, value);
    //    }

    //    public float MoveSpeed {
    //        get => moveSpeed;
    //        set => moveSpeed = Mathf.Max(MOVE_SPEED_MIN, value);
    //    }

    //    public float FlyMoveSpeed {
    //        get => airMoveModifier;
    //        set => Mathf.Max(FLY_MOVE_SPEED_MIN, value);
    //    }

    //    public float JumpForce {
    //        get => jumpForce;
    //        set => jumpForce = Mathf.Max(JUMP_FORCE_MIN, value);
    //    }

    //    protected float gravity => Physics.gravity.y;

    //    protected override void Awake()
    //    {
    //        base.Awake();
    //        collider = rigidbody.Q().Component<CapsuleCollider>().Strict();
    //        originalLinearDampging = rigidbody.linearDamping;
    //    }

    //    protected virtual void FixedUpdate()
    //    {
    //        RaycastSurface();
    //        HandleMovement();
    //    }

    //    public void Move(
    //        Vector3 dir
    //        )
    //    {
    //        moveDirection = dir;
    //    }

    //    public void MoveByInput(Vector2 inputValue)
    //    {
    //        var dir = new Vector3(inputValue.x, 0f, inputValue.y);

    //        Move(dir);
    //    }

    //    public void Jump()
    //    {
    //        jumpRequested = true;
    //    }

    //    private void RaycastSurface()
    //    {
    //        Vector3 colliderCenter = transform.TransformPoint(collider.center);
    //        float rayStartOffset = (collider.height * 0.5f) - collider.radius;
    //        Vector3 rayStart = colliderCenter + Vector3.up * rayStartOffset;

    //        float castRadius = Mathf.Max(0.01f, collider.radius * 0.9f);
    //        float rayLength = collider.radius + surfaceCastDistance;

    //        if (Physics.SphereCast(
    //            rayStart,
    //            castRadius,
    //            Vector3.down,
    //            out RaycastHit hit,
    //            rayLength, 
    //            surfaceCastLayerMask.Deserialized ?? Physics.AllLayers
    //            ))
    //        {
    //            float angle = Vector3.Angle(hit.normal, Vector3.up);

    //            if (angle <= maxSlopeAngle)
    //            {
    //                coyoteTimeCounter = coyoteTime;
    //                if (!isGrounded) isGrounded = true;
    //                return;
    //            }
    //        }

    //        if (isGrounded)
    //        {
    //            coyoteTimeCounter -= Time.fixedDeltaTime;
    //            if (coyoteTimeCounter <= 0)
    //            {
    //                isGrounded = false;
    //                coyoteTimeCounter = 0;
    //            }
    //        }
    //    }

    //    private void OnDrawGizmos()
    //    {
    //        if (collider == null) return;

    //        Vector3 colliderCenter = transform.TransformPoint(collider.center);
    //        float rayStartOffset = (collider.height * 0.5f) - collider.radius;
    //        Vector3 rayStart = colliderCenter + Vector3.up * rayStartOffset;
    //        float rayLength = collider.radius + surfaceCastDistance;

    //        // Рисуем точку начала луча (Красная сфера)
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawSphere(rayStart, 0.1f);

    //        // Рисуем сам луч (Желтая линия)
    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawLine(rayStart, rayStart + Vector3.down * rayLength);

    //        // Рисуем зону проверки земли (Зеленая сфера внизу)
    //        Gizmos.color = Color.green;
    //        Gizmos.DrawWireSphere(rayStart + Vector3.down * rayLength, 1f);
    //    }

    //    private void HandleMovement()
    //    {
    //        var velocity = rigidbody.linearVelocity;

    //        if (float.IsNaN(velocity.x)
    //            ||
    //            float.IsNaN(velocity.y)
    //            ||
    //            float.IsNaN(velocity.z))
    //        {
    //            rigidbody.linearVelocity = Vector3.zero;
    //            return;
    //        }

    //        if (isGrounded)
    //        {
    //            if (velocity.y < 0.5f)
    //                velocity = velocity.WithY(0f);

    //            rigidbody.linearVelocity = velocity;
    //            rigidbody.linearDamping = originalLinearDampging;
    //        }
    //        else
    //        {
    //            velocity += gravity * Time.fixedDeltaTime * Vector3.up;

    //            if (velocity.y < 0f)
    //                velocity += gravity * 0.5f * Time.fixedDeltaTime * Vector3.up;

    //            rigidbody.linearVelocity = velocity;
    //            rigidbody.linearDamping = originalLinearDampging * 2f;
    //        }

    //        float moveDirMagnitude;

    //        if (moveDirection.sqrMagnitude > 1f)
    //        {
    //            moveDirection.Normalize();
    //            moveDirMagnitude = 1f;
    //        }
    //        else
    //            moveDirMagnitude = moveDirection.magnitude;

    //        var targetSpeed = moveDirMagnitude * moveSpeed;
    //        var currenAccl = moveDirMagnitude > 0f ? acceleration : deceleration;

    //        if (!isGrounded)
    //            currenAccl *= airMoveModifier;

    //        var targetVelocity = moveDirection * moveSpeed;

    //        var newHorVelocity = Vector3.Lerp(
    //            velocity.WithY(0f),
    //            targetVelocity,
    //            currenAccl * Time.fixedDeltaTime
    //            );

    //        rigidbody.linearVelocity = newHorVelocity.WithY(velocity.y);
    //    }
    //}
}
