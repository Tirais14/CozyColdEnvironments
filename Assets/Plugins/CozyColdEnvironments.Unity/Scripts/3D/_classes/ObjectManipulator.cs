using CCEnvs.Unity.Components;
using CCEnvs.Unity.EditorSerialization;
using CCEnvs.Unity.Snapshots;
using Unity.Cinemachine;
using UnityEngine;
using OffsetOptions = CCEnvs.Unity.D3.ObjectManipulatorOffsetSettings;
using Options = CCEnvs.Unity.D3.ObjectManipulatorSettings;
using VelocityOptions = CCEnvs.Unity.D3.ObjectManipulatorVelocitySettings;

#nullable enable
namespace CCEnvs.Unity.D3
{
    public sealed partial class ObjectManipulator : CCBehaviour
    {
        public const float OBJECT_MOVE_SENSIVITY_MIN = 0f;
        public const float OBJECT_MOVE_DAMPING_MIN = 0f;
        public const float OBJECT_ROTATION_SENSIVITY_MIN = 0f;
        public const float OBJECT_ROTATION_DAMPING_MIN = 0f;
        public const float MANIPULATOR_OBJECT_DISTANCE_OFFSET_MIN = 0.1f;
        public const float MANIPULATOR_OBJECT_HORIZONTAL_OFFSET_MIN = 0f;
        public const float MANIPULATOR_OBJECT_VERTICAL_OFFSET_MIN = 0f;
        public const float MANIPULATOR_OBJECT_PITCH_OFFSET_MIN = 0f;
        public const float MANIPULATOR_OBJECT_YAW_OFFSET_MIN = 0f;
        public const float MANIPULATOR_OBJECT_ROLL_OFFSET_MIN = 0f;

        public const long COLLISTION_DETECTION_EVERY_FRAME_MIN = 1L;

        [Header("Behaviour Settings")]
        [Space(6f)]

        [SerializeField, Min(COLLISTION_DETECTION_EVERY_FRAME_MIN)]
        private long collisionDetectionEveryFrame = 2L;

        [SerializeField]
        private Options settings = Options.Default;

        [SerializeField]
        private OffsetOptions offsetSettings = OffsetOptions.Default;

        [SerializeField]
        private VelocityOptions velocitySettings = VelocityOptions.Default;

        [Header("Object Settings")]
        [Space(6f)]

        [SerializeField, Min(MANIPULATOR_OBJECT_DISTANCE_OFFSET_MIN)]
        private float manipulatorObjectDistanceOffset = 1f;

        [SerializeField, Min(MANIPULATOR_OBJECT_HORIZONTAL_OFFSET_MIN)]
        private float manipulatorObjectHorizontalOffset = 0f;

        [SerializeField, Min(MANIPULATOR_OBJECT_VERTICAL_OFFSET_MIN)]
        private float manipulatorObjectVerticalOffset = 0f;

        [SerializeField, Min(MANIPULATOR_OBJECT_PITCH_OFFSET_MIN)]
        private float manipulatorObjectPitchOffset = 0f;

        [SerializeField, Min(MANIPULATOR_OBJECT_YAW_OFFSET_MIN)]
        private float manipulatorObjectYawOffset = 0f;

        [SerializeField, Min(MANIPULATOR_OBJECT_ROLL_OFFSET_MIN)]
        private float manipulatorObjectRollOffset = 0f;

        [Header("Surface Colliding Settings")]
        [Space(6f)]

        [SerializeField]
        private SerializedNullable<LayerMask> surfaceCastMask;

        [Header("Move Settings")]
        [Space(6f)]

        [SerializeField, Min(OBJECT_MOVE_SENSIVITY_MIN)]
        private float objectMoveSensivity = 1f;

        [SerializeField, Min(OBJECT_MOVE_DAMPING_MIN)]
        private float objectMoveDamping = 20f;

        [SerializeField, Min(OBJECT_ROTATION_SENSIVITY_MIN)]
        private float objectRotationSensivity = 1f;

        [SerializeField, Min(OBJECT_ROTATION_DAMPING_MIN)]
        private float objectRotationDamping = 20f;

        private Vector3 ojbPos;
        private Vector3 objPosOffset;

        private Vector3? maniputalorObjPosOffset;
        private Vector3? maniputalorObjRotOffset;

        private Quaternion objRot;
        private Quaternion objRotOffset;

        private float? objRadius;

        private RigidBodySnapshot objectSnapshot = new();

        public Rigidbody? Object { get; private set; }

        public Collider? ObjectCollider { get; private set; }

        public Options Settings => settings;

        public OffsetOptions OffsetSettings => offsetSettings;

        public VelocityOptions VelocitySettings => velocitySettings;

        public LayerMask SurfaceCastMask => surfaceCastMask.Deserialized ?? Physics.AllLayers;

        public float ObjectMoveSensitivity => objectMoveSensivity;
        public float ObjectMoveDamping => objectMoveDamping;
        public float ObjectRotationSensitivity => objectRotationSensivity;
        public float ObjectRotationDamping => objectRotationDamping;

        public long UpdateEveryFrame => collisionDetectionEveryFrame;

        public Vector3 ManipulatorObjectPositionOffset {
            get
            {
                maniputalorObjPosOffset ??= new Vector3(manipulatorObjectHorizontalOffset, manipulatorObjectVerticalOffset, manipulatorObjectDistanceOffset);
                return maniputalorObjPosOffset.Value;
            }
        }
        public Vector3 ManipulatorObjectRotationOffset {
            get
            {
                maniputalorObjRotOffset ??= new Vector3(manipulatorObjectPitchOffset, manipulatorObjectYawOffset, manipulatorObjectRollOffset);
                return maniputalorObjRotOffset.Value;
            }
        }
        public Vector3 ObjectPosition => ojbPos;
        public Vector3 ObjectPositionOffset => objPosOffset;

        public Quaternion ObjectRotation => objRot;
        public Quaternion ObjectRotationOffset => objRotOffset;

        private void FixedUpdate()
        {
            HandleObjectTransform();
        }

#if UNITY_EDITOR && CC_DEBUG_ENABLED
        private void OnDrawGizmos()
        {
            if (Object == null)
                return;

            Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(Object.position, GetObjectRadius());
        }
#endif

#nullable disable warnings
        public float GetObjectRadius()
        {
            if (objRadius != null
                &&
                settings.IsFlagSetted(Options.ObjectSizeChangeable))
            {
                return objRadius.Value;
            }

            var extents = ObjectCollider.bounds.extents;

            if (!ValidateVector3(extents, nameof(extents)))
                return 0.5f;

            float bigger = 0f;

            float value;

            for (int i = 0; i < 3; i++)
            {
                value = extents[i];

                if (value > bigger)
                    bigger = value;
            }

            objRadius = bigger;

            return objRadius.Value;
        }
        private void OnDropObject()
        {
            objectSnapshot.TryRestore(Object);

            if (velocitySettings.IsFlagSetted(VelocityOptions.ResetOnDrop))
            {
                Object.linearVelocity = Vector3.zero;
                Object.angularVelocity = Vector3.zero;
            }

            objRadius = null;
        }

        private void SetObjectCollider()
        {
            ObjectCollider = Object.GetComponent<Collider>();
            GetObjectRadius();
        }

        private void OnSetObject()
        {
            ojbPos = Object.transform.position;
            objRot = Object.transform.rotation;

            objectSnapshot.CaptureFrom(Object);
            objectSnapshot.SetLinearVelocity(null)
                .SetAngularVelocity(null);

            Object.useGravity = false;
            Object.isKinematic = true;
            Object.collisionDetectionMode = CollisionDetectionMode.Continuous;

            if (velocitySettings.IsFlagSetted(VelocityOptions.ResetOnSet))
            {
                Object.linearVelocity = Vector3.zero;
                Object.angularVelocity = Vector3.zero;
            }

            if (offsetSettings.IsFlagSetted(OffsetOptions.ResetOnObjectChanged))
                ResetPositionOffset().ResetRotationOffset();

            SetObjectCollider();
        }

        private bool TrySurfaceCast(out Vector3 surfacePoint, out Vector3 surfaceNormal)
        {
            surfacePoint = transform.position;
            surfaceNormal = transform.forward;

            var maxCastDistance = manipulatorObjectDistanceOffset + 0.5f;

            if (Physics.Raycast(
                transform.position,
                transform.forward,
                out var hit,
                maxCastDistance,
                SurfaceCastMask,
                QueryTriggerInteraction.Ignore
                ))
            {
                surfacePoint = hit.point;
                surfaceNormal = hit.normal;
                return true;
            }

            return false;
        }

        private void HandleCollisions(ref Vector3 targetPos)
        {
            var moveDir = targetPos - Object.position;
            var moveDirNormalized = moveDir.normalized;
            var moveDistance = moveDir.magnitude;
            var objRadius = GetObjectRadius();
            var minCastDistance = objRadius * 0.5f;

            if (moveDistance < minCastDistance)
                return;

            float maxCastDistance = moveDistance + objRadius * 0.7f;

            if (Physics.SphereCast(
                Object.position,
                objRadius,
                moveDirNormalized,
                out var hit,
                maxCastDistance,
                SurfaceCastMask,
                QueryTriggerInteraction.Ignore
                ))
            {
                targetPos = hit.point + hit.normal * (objRadius * 1.02f);

                float toSurfaceDistance = hit.distance;
                float remainingMoveDistance = moveDistance - toSurfaceDistance;

                if (remainingMoveDistance > 0.01f)
                {
                    Vector3 slideDir = Vector3.ProjectOnPlane(moveDirNormalized, hit.normal);
                    targetPos += slideDir * (remainingMoveDistance * 0.9f);

                    HandleCollisions(ref targetPos);
                }
            }
        }

        private readonly Collider[] _colliderBuffer = new Collider[32];

        private bool TryCompurePentration(
            in Vector3 targetPos,
            Collider col,
            out Vector3 pushDir,
            out float depth
            )
        {
            return Physics.ComputePenetration(
                ObjectCollider,
                targetPos,
                Object.transform.rotation,
                col,
                col.transform.position,
                col.transform.rotation,
                out pushDir,
                out depth
                );
        }

        private Vector3 GetPushDirection(
            in Vector3 targetPos,
            int overlappedCount
            )
        {
            if (overlappedCount < 1)
                return Vector3.zero;

            Vector3 combinedNormal = Vector3.zero;

            Collider col;

            for (int i = 0; i < overlappedCount; i++)
            {
                col = _colliderBuffer[i];

                if (TryCompurePentration(
                    targetPos,
                    col,
                    out var pushDir,
                    out var depth
                    ))
                {
                    combinedNormal += pushDir;
                }
            }

            if (combinedNormal != Vector3.zero)
                combinedNormal.Normalize();

            return combinedNormal;
        }

        private void PushObjectFromOther(
            ref Vector3 targetPos,
            float radius,
            in Vector3 pushDir
            )
        {
            if (pushDir == Vector3.zero)
                return;

            const float PUSH_OFFSET = 0.1f;
            targetPos += pushDir * (radius * PUSH_OFFSET);
        }

        private bool TryOverlapNearestObjects(
            in Vector3 targetPos,
            float radius,
            out int overlappedCount
            )
        {
            overlappedCount = Physics.OverlapSphereNonAlloc(
                targetPos,
                radius,
                _colliderBuffer,
                SurfaceCastMask,
                QueryTriggerInteraction.Ignore
                );

            return overlappedCount > 0;
        }

        private void TryPushObjectFromOther(ref Vector3 targetPos)
        {
            float radius = GetObjectRadius() * 0.95f;

            if (!TryOverlapNearestObjects(targetPos, radius, out var overlappedCount))
                return;

            Vector3 pushDir = GetPushDirection(targetPos, overlappedCount);

            PushObjectFromOther(ref targetPos, radius, pushDir);
        }

#nullable enable warnings

        private bool ValidateVector3(in Vector3 value, string? name = null)
        {
            if (!value.IsFinite())
            {
                var msg = $"Invalid {nameof(Vector3)}. {nameof(Vector3)}: {value}";

                if (name is not null)
                    msg += $"; name: {name}";

                this.PrintError(msg);

                return false;
            }

            return true;
        }

        private bool ValidateQuaternion(in Quaternion value, string? name = null)
        {
            if (!value.IsFinite())
            {
                var msg = $"Invalid {nameof(Quaternion)}. {nameof(Quaternion)}: {value}";

                if (name is not null)
                    msg += $"; name: {name}";

                this.PrintError(msg);

                return false;
            }

            return true;
        }

        private bool ValidateManipulatorPosition()
        {
            return ValidateVector3(transform.position, "transform.position");
        }

        private bool ValidateObjectPosition()
        {
            return ValidateVector3(ojbPos, nameof(ojbPos));
        }

        private bool ValidateObjectRotation()
        {
            return ValidateQuaternion(objRot, nameof(objRot));
        }

        private Vector3 CalcutaeTargetPositionRelativeToSurface(
            in Vector3 surfacePoint,
            in Vector3 surfaceNormal
            )
        {
            var tangent = Vector3.ProjectOnPlane(transform.right, surfaceNormal).normalized;
            var binormal = Vector3.ProjectOnPlane(transform.up, surfaceNormal).normalized;

            return surfacePoint
                   +
                   surfaceNormal * manipulatorObjectDistanceOffset
                   +
                   tangent * manipulatorObjectHorizontalOffset
                   +
                   binormal * manipulatorObjectVerticalOffset;
        }

        //private bool TrySurfaceCast(out RaycastHit hit)
        //{
        //    return Physics.Raycast(

        //        );
        //}

        //private void ClampObjectMoveDistanceBySurface(ref Vector3 targetPos)
        //{
        //    if (TrySurfaceCast())
        //}

        private Vector3 ResolveTargetPosition()
        {
            if (settings.IsFlagSetted(Options.CollideWithSurface)
                &&
                TrySurfaceCast(out var surfacePoint, out var surfaceNormal))
            {
                return CalcutaeTargetPositionRelativeToSurface(surfacePoint, surfaceNormal);
            }

            return transform.position + transform.TransformDirection(ManipulatorObjectPositionOffset + objPosOffset);
        }

        private void HandleObjectTransform()
        {
            if (Object == null)
                return;

            Vector3 targetPos = ResolveTargetPosition();
            Quaternion targetRot = transform.rotation * Quaternion.Euler(ManipulatorObjectRotationOffset) * objRotOffset;

            if (settings.IsFlagSetted(Options.CollideWithSurface))
            {
                TryPushObjectFromOther(ref targetPos);
                HandleCollisions(ref targetPos);
            }

            float moveT = 1f - Mathf.Exp(-objectMoveDamping * Time.fixedDeltaTime);
            float rotT = 1f - Mathf.Exp(-objectRotationDamping * Time.fixedDeltaTime);

            ojbPos = Vector3.Lerp(ojbPos, targetPos, moveT);

            if (!ValidateObjectPosition())
            {
                ojbPos = Object.position;
                return;
            }

            objRot = Quaternion.Slerp(objRot, targetRot, rotT);

            if (!ValidateObjectRotation())
            {
                objRot = Object.rotation;
                return;
            }

            if (Time.frameCount % 20L == 0) //Reset calcualtions error to avoid NaN
                objRot.Normalize();

            Object.Move(ojbPos, objRot);
        }
    }
}
