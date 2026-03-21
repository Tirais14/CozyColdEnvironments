using CCEnvs.Unity.Components;
using CCEnvs.Unity.EditorSerialization;
using CCEnvs.Unity.Snapshots;
using UnityEngine;
using ObjectOptions = CCEnvs.Unity.D3.ObjectManipulatorObjectSettings;
using OffsetOptions = CCEnvs.Unity.D3.ObjectManipulatorOffsetSettings;
using VelocityOptions = CCEnvs.Unity.D3.ObjectManipulatorVelocitySettings;

#nullable enable
namespace CCEnvs.Unity.D3
{
    [DisallowMultipleComponent]
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

        private readonly RigidBodySnapshot objectSnapshot = new();

        [Header("Behaviour Settings")]
        [Space(6f)]

        [SerializeField]
        private ObjectOptions objectSettings = ObjectOptions.Default;

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

        public Rigidbody? Object { get; private set; }

        public Collider? ObjectCollider { get; private set; }

        public ObjectOptions ObjectSettings => objectSettings;

        public OffsetOptions OffsetSettings => offsetSettings;

        public VelocityOptions VelocitySettings => velocitySettings;

        public LayerMask SurfaceCastMask => surfaceCastMask.Deserialized ?? Physics.AllLayers;

        public float ObjectMoveSensitivity => objectMoveSensivity;
        public float ObjectMoveDamping => objectMoveDamping;
        public float ObjectRotationSensitivity => objectRotationSensivity;
        public float ObjectRotationDamping => objectRotationDamping;

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
                objectSettings.IsFlagSetted(ObjectOptions.ObjectSizeChangeable))
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
            Object.excludeLayers = 1 << gameObject.layer;
            Object.collisionDetectionMode = CollisionDetectionMode.Continuous;
            Object.interpolation = RigidbodyInterpolation.Interpolate;

            if (velocitySettings.IsFlagSetted(VelocityOptions.ResetOnSet))
            {
                Object.linearVelocity = Vector3.zero;
                Object.angularVelocity = Vector3.zero;
            }

            if (offsetSettings.IsFlagSetted(OffsetOptions.ResetOnObjectChanged))
                ResetPositionOffset().ResetRotationOffset();

            SetObjectCollider();
        }

        private void HandleCollisions(
            ref Vector3 targetPos,
            int recursionDepth = 0
            )
        {
            if (recursionDepth > 6)
            {
                this.PrintWarning($"Prevented recursion call of {nameof(HandleCollisions)}");
                return;
            }

            var moveDir = targetPos - Object.position;
            var moveDirNormalized = moveDir.normalized;
            var moveDistance = moveDir.magnitude;
            var objRadius = GetObjectRadius();
            var minCastDistance = objRadius * 0.5f;

            if (moveDistance < minCastDistance)
                return;

            float maxCastDistance = moveDistance + objRadius * 0.5f;

            const float SURFACE_PENETRATION_PREVENT_RADIUS_OFFSET = 0.99f;

            if (Physics.SphereCast(
                Object.position,
                objRadius * SURFACE_PENETRATION_PREVENT_RADIUS_OFFSET,
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

                    HandleCollisions(ref targetPos, ++recursionDepth);
                }
            }
        }

        private bool EnsureObjectIsVisible(ref Vector3 targetPos)
        {
            var dirToObj = targetPos - transform.position;
            var distanceToObj = dirToObj.magnitude;

            if (distanceToObj < 0.01f)
                return false;

            var dirToObjNormalized = dirToObj.normalized;
            var objRadius = GetObjectRadius();

            if (Physics.SphereCast(
                transform.position,
                objRadius * 0.5f,
                dirToObjNormalized,
                out var hit,
                distanceToObj,
                SurfaceCastMask,
                QueryTriggerInteraction.Ignore
                )
                &&
                hit.rigidbody != Object
                &&
                !hit.transform.IsChildOf(Object.transform)
                )
            {
                targetPos = hit.point + hit.normal * (objRadius * 1.02f);

                return true;
            }

            return false;
        }

#nullable enable warnings

        private void HandleObjectTransform()
        {
            if (Object == null)
                return;

            Vector3 targetPos = transform.position + transform.TransformDirection(ManipulatorObjectPositionOffset + objPosOffset);
            Quaternion targetRot = transform.rotation * Quaternion.Euler(ManipulatorObjectRotationOffset) * objRotOffset;

            if (objectSettings.IsFlagSetted(ObjectOptions.CollideWithSurface))
                HandleCollisions(ref targetPos);

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

            if (Time.frameCount % 20L == 0) //Reset calcualtion errors to avoid NaN
                objRot.Normalize();

            Object.Move(ojbPos, objRot);
        }

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
    }
}
