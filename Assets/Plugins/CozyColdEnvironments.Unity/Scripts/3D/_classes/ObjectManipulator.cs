using CCEnvs.Unity.Components;
using CCEnvs.Unity.EditorSerialization;
using CCEnvs.Unity.Snapshots;
using CommunityToolkit.Diagnostics;
using System;
using UnityEngine;
using ObjectOptions = CCEnvs.Unity.D3.ObjectManipulatorObjectSettings;
using OffsetOptions = CCEnvs.Unity.D3.ObjectManipulatorOffsetSettings;
using VelocityOptions = CCEnvs.Unity.D3.ObjectManipulatorVelocitySettings;

#nullable enable
namespace CCEnvs.Unity.D3
{
    [DisallowMultipleComponent]
    public sealed class ObjectManipulator : CCBehaviour
    {
        public const float MOVE_SENSIVITY_MIN = 0f;
        public const float MOVE_DAMPING_MIN = 0f;
        public const float ROTATION_SENSIVITY_MIN = 0f;
        public const float ROTATION_DAMPING_MIN = 0f;
        public const float PIVOT_DISTANCE_OFFSET_MIN = 0.1f;
        public const float PIVOT_HORIZONTAL_OFFSET_MIN = 0f;
        public const float PIVOT_VERTICAL_OFFSET_MIN = 0f;
        public const float PIVOT_PITCH_OFFSET_MIN = 0f;
        public const float PIVOT_YAW_OFFSET_MIN = 0f;
        public const float PIVOT_ROLL_OFFSET_MIN = 0f;

        private readonly RigidbodySnapshot objectSnapshot = new();

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

        [SerializeField, Min(PIVOT_DISTANCE_OFFSET_MIN)]
        private float pivotDistanceOffset = 1f;

        [SerializeField, Min(PIVOT_HORIZONTAL_OFFSET_MIN)]
        private float pivotHorizontalOffset = 0f;

        [SerializeField, Min(PIVOT_VERTICAL_OFFSET_MIN)]
        private float pivotVerticalOffset = 0f;

        [SerializeField, Min(PIVOT_PITCH_OFFSET_MIN)]
        private float pivotPitchOffset = 0f;

        [SerializeField, Min(PIVOT_YAW_OFFSET_MIN)]
        private float pitvotYawOffset = 0f;

        [SerializeField, Min(PIVOT_ROLL_OFFSET_MIN)]
        private float pivotRollOffset = 0f;

        [Header("Surface Colliding Settings")]
        [Space(6f)]

        [SerializeField]
        private SerializedNullable<LayerMask> surfaceCastMask;

        [Header("Move Settings")]
        [Space(6f)]

        [SerializeField, Min(MOVE_SENSIVITY_MIN)]
        private float moveSensivity = 1f;

        [SerializeField, Min(MOVE_DAMPING_MIN)]
        private float moveDamping = 20f;

        [SerializeField, Min(ROTATION_SENSIVITY_MIN)]
        private float rotationSensivity = 1f;

        [SerializeField, Min(ROTATION_DAMPING_MIN)]
        private float rotationDamping = 20f;

        private Vector3 objPos;
        private Vector3 objPosOffset;

        private Vector3? pivotPosOffset;
        private Vector3? pivotRotOffset;

        private Quaternion objRot;
        private Quaternion objRotOffset;

        private float? objRadius;

        private Rigidbody? obj;

        public Rigidbody? Object {
            get => obj;
            set => SetObject(value);
        }

        public Collider? ObjectCollider { get; private set; }

        public ObjectOptions ObjectSettings => objectSettings;

        public OffsetOptions OffsetSettings => offsetSettings;

        public VelocityOptions VelocitySettings => velocitySettings;

        public LayerMask SurfaceCastMask => surfaceCastMask.Deserialized ?? Physics.AllLayers;

        public float MoveSensitivity => moveSensivity;
        public float MoveDamping => moveDamping;
        public float RotationSensitivity => rotationSensivity;
        public float RotationDamping => rotationDamping;

        public Vector3 PivotPositionOffset {
            get
            {
                pivotPosOffset ??= new Vector3(pivotHorizontalOffset, pivotVerticalOffset, pivotDistanceOffset);
                return pivotPosOffset.Value;
            }
        }
        public Vector3 PivotRotationOffset {
            get
            {
                pivotRotOffset ??= new Vector3(pivotPitchOffset, pitvotYawOffset, pivotRollOffset);
                return pivotRotOffset.Value;
            }
        }
        public Vector3 ObjectPosition => objPos;
        public Vector3 ObjectPositionOffset => objPosOffset;

        public Quaternion ObjectRotation => objRot;
        public Quaternion ObjectRotationOffset => objRotOffset;

        private Vector3? ColliderCenter {
            get
            {
                if (ObjectCollider == null)
                    return null;

                return ObjectCollider.bounds.center;
            }
        }

        private Vector3? ColliderOffset {
            get
            {
                if (Object == null)
                    return Vector3.zero;

                return ColliderCenter - Object.position;
            }
        }

        private void FixedUpdate()
        {
            HandleObjectTransform();
        }

#if UNITY_EDITOR && CC_DEBUG_ENABLED
        private void OnDrawGizmos()
        {
            //if (Object == null)
            //    return;

            //Gizmos.color = Color.green;

            //Gizmos.DrawWireSphere(ObjectCollider.bounds.center - Object.position, GetObjectRadius());

            if (ObjectCollider == null || Object == null) return;

            // Пивот Rigidbody (красный)
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(Object.position, 0.05f);

            // Центр коллайдера (зелёный)
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(ObjectCollider.bounds.center, 0.05f);

            // Вектор смещения (жёлтый)
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(Object.position, ObjectCollider.bounds.center);

            // Сфера коллизии (синяя)
            Gizmos.color = new Color(0, 0, 1, 0.3f);
            Gizmos.DrawWireSphere(ObjectCollider.bounds.center, GetObjectRadius());

            // Текущая расчётная позиция (маджента)
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(objPos, 0.1f);
        }
#endif

        #region Setters
        public ObjectManipulator Configure(Action<ObjectManipulator> configurer)
        {
            Guard.IsNotNull(configurer, nameof(configurer));

            configurer(this);

            return this;
        }

        public ObjectManipulator SetObject(Rigidbody? value)
        {
            if (Object != null && value == Object)
                return this;

            if (Object != null)
            {
                OnDropObject();
                obj = null;
            }

            if (value != null)
            {
                obj = value;
                OnSetObject();
            }

            return this;
        }

        public ObjectManipulator SetPivotPitchOffset(float value)
        {
            pivotPitchOffset = Mathf.Max(value, PIVOT_PITCH_OFFSET_MIN);
            pivotRotOffset = null;
            return this;
        }

        public ObjectManipulator SetPivotYawOffset(float value)
        {
            pitvotYawOffset = Mathf.Max(value, PIVOT_YAW_OFFSET_MIN);
            pivotRotOffset = null;
            return this;
        }

        public ObjectManipulator SetPivotRollOffset(float value)
        {
            pivotRollOffset = Mathf.Max(value, PIVOT_ROLL_OFFSET_MIN);
            pivotRotOffset = null;
            return this;
        }

        public ObjectManipulator SetPivotDistanceOffset(float value)
        {
            pivotDistanceOffset = Mathf.Max(value, PIVOT_DISTANCE_OFFSET_MIN);
            pivotPosOffset = null;
            return this;
        }

        public ObjectManipulator SetPivotHorizontalOffset(float value)
        {
            pivotHorizontalOffset = Mathf.Max(value, PIVOT_HORIZONTAL_OFFSET_MIN);
            pivotPosOffset = null;
            return this;
        }

        public ObjectManipulator SetPivotVerticalOffset(float value)
        {
            pivotVerticalOffset = Mathf.Max(value, PIVOT_VERTICAL_OFFSET_MIN);
            pivotPosOffset = null;
            return this;
        }

        public ObjectManipulator SetPositionOffset(Vector3 posOffset)
        {
            objPosOffset = posOffset;

            return this;
        }

        public ObjectManipulator SetRotationOffset(Quaternion rotOffset)
        {
            objRotOffset = rotOffset;

            return this;
        }

        public ObjectManipulator ResetPositionOffset()
        {
            objPosOffset = Vector3.zero;

            return this;
        }

        public ObjectManipulator ResetRotationOffset()
        {
            objRotOffset = Quaternion.identity;

            return this;
        }

        public ObjectManipulator SetMoveSensitivity(float value)
        {
            moveSensivity = Mathf.Max(value, MOVE_SENSIVITY_MIN);
            return this;
        }

        public ObjectManipulator SetMoveDamping(float value)
        {
            moveDamping = Mathf.Max(value, MOVE_DAMPING_MIN);
            return this;
        }

        public ObjectManipulator SetRotationSensitivity(float value)
        {
            rotationSensivity = Mathf.Max(value, ROTATION_SENSIVITY_MIN);
            return this;
        }

        public ObjectManipulator SetRotationDamping(float value)
        {
            rotationDamping = Mathf.Max(value, ROTATION_DAMPING_MIN);
            return this;
        }

        public ObjectManipulator SetSurfaceCastMask(LayerMask? mask)
        {
            surfaceCastMask = new SerializedNullable<LayerMask>(mask);
            return this;
        }

        public ObjectManipulator SetSettings(ObjectOptions value)
        {
            objectSettings = value;
            return this;
        }

        public ObjectManipulator SetOffsetSettings(OffsetOptions value)
        {
            offsetSettings = value;
            return this;
        }

        public ObjectManipulator SetVelocitySettings(VelocityOptions value)
        {
            velocitySettings = value;
            return this;
        }

        public ObjectManipulator AddPositionOffset(Vector3 delta)
        {
            objPosOffset += delta;
            return this;
        }

        public ObjectManipulator AddRotationOffset(Quaternion delta)
        {
            objRotOffset *= delta;
            return this;
        }

        public ObjectManipulator AddRotationOffsetEuler(Vector3 deltaEuler)
        {
            objRotOffset *= Quaternion.Euler(deltaEuler);
            return this;
        }

        public ObjectManipulator ApplyTranslation(Vector3 deltaPos)
        {
            objPosOffset += deltaPos * moveSensivity;

            return this;
        }

        public ObjectManipulator ApplyRotation(Vector3 deltaEuler)
        {
            var rotation = deltaEuler * rotationSensivity;

            objRotOffset *= Quaternion.Euler(rotation);

            return this;
        }

        #endregion Setters

#nullable disable warnings
        public float GetObjectRadius()
        {
            if (objRadius != null
                &&
                objectSettings.HasFlagT(ObjectOptions.ObjectSizeChangeable))
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

            if (velocitySettings.HasFlagT(VelocityOptions.ResetOnDrop))
            {
                Object.linearVelocity = Vector3.zero;
                Object.angularVelocity = Vector3.zero;
            }

            objRadius = null;
        }

        private void SetObjectComponents()
        {
            ObjectCollider = Object.GetComponent<Collider>();
            GetObjectRadius();
        }

        private void OnSetObject()
        {
            objRot = Object.transform.rotation;

            objectSnapshot.CaptureFrom(Object);
            objectSnapshot.SetLinearVelocity(null)
                .SetAngularVelocity(null);

            Object.useGravity = false;
            Object.isKinematic = true;
            Object.excludeLayers = 1 << gameObject.layer;
            Object.collisionDetectionMode = CollisionDetectionMode.Continuous;
            Object.interpolation = RigidbodyInterpolation.Interpolate;

            if (velocitySettings.HasFlagT(VelocityOptions.ResetOnSet))
            {
                Object.linearVelocity = Vector3.zero;
                Object.angularVelocity = Vector3.zero;
            }

            if (offsetSettings.HasFlagT(OffsetOptions.ResetOnObjectChanged))
                ResetPositionOffset().ResetRotationOffset();

            SetObjectComponents();

            objPos = ObjectCollider.bounds.center;

            var surfaceOffset = CorrectObjectPositionRelativeToSurface();
            objPos += surfaceOffset;
        }

        private void HandleObjectTransform()
        {
            if (Object == null || ObjectCollider == null)
                return;

            Vector3 colliderOffset = ColliderOffset.Value;

            Vector3 targetPivotPos = GetTransformBasedTargetPosition();

            Quaternion targetRot = transform.rotation * Quaternion.Euler(PivotRotationOffset) * objRotOffset;

            Vector3 targetColliderPos = targetPivotPos + colliderOffset;

            if (objectSettings.HasFlagT(ObjectOptions.CollideWithSurface))
                HandleCollisions(ref targetColliderPos);

            float moveT = 1f - Mathf.Exp(-moveDamping * Time.fixedDeltaTime);
            float rotT = 1f - Mathf.Exp(-rotationDamping * Time.fixedDeltaTime);

            objPos = Vector3.Lerp(objPos, targetColliderPos, moveT);
            objRot = Quaternion.Slerp(objRot, targetRot, rotT);

            if (Time.frameCount % 20L == 0) 
                objRot.Normalize();

            Vector3 finalPivotPos = objPos - colliderOffset;

            Object.Move(finalPivotPos, objRot);
        }

        private Vector3 CorrectObjectPositionRelativeToSurface()
        {
            Vector3 colliderPos = ObjectCollider.bounds.center;
            Vector3 targetPivotPos = GetTransformBasedTargetPosition();

            Vector3 toTargetDir = targetPivotPos - colliderPos;
            Vector3 toTargetDirNormalized = toTargetDir.normalized;

            if (Physics.Raycast(
                colliderPos,
                toTargetDirNormalized,
                out var hit,
                GetObjectRadius() * 2f,
                SurfaceCastMask,
                QueryTriggerInteraction.Ignore
                )
                &&
                hit.collider != ObjectCollider)
            {
                if (hit.collider is TerrainCollider)
                    return hit.normal * (GetObjectRadius() * toTargetDir.magnitude  * 3f);
                else
                    return hit.normal * (GetObjectRadius() * 2f);
            }
            return Vector3.zero;
        }

        private void HandleCollisions(
            ref Vector3 targetColliderPos,
            int recursionDepth = 0
            )
        {
            if (recursionDepth > 6)
                return;

            Vector3 colliderPos = ObjectCollider.bounds.center;
            Vector3 moveDir = targetColliderPos - colliderPos;
            float moveDistance = moveDir.magnitude;

            if (moveDistance < 0.001f)
                return;

            Vector3 moveDirNormalized = moveDir.normalized;
            float objRadius = GetObjectRadius();
            float maxCastDistance = moveDistance + objRadius;

            if (Physics.SphereCast(
                colliderPos,
                objRadius * 0.99f,
                moveDirNormalized,
                out var hit,
                maxCastDistance,
                SurfaceCastMask,
                QueryTriggerInteraction.Ignore
                ))
            {
                Vector3 correctedPoint = hit.point;

                if (hit.collider is TerrainCollider)
                    correctedPoint += hit.normal * 0.01f; 

                targetColliderPos = correctedPoint + hit.normal * (objRadius * 1.02f);

                float remainingMoveDistance = moveDistance - hit.distance;

                if (remainingMoveDistance > 0.01f)
                {
                    Vector3 slideDir = Vector3.ProjectOnPlane(moveDirNormalized, hit.normal);
                    targetColliderPos += slideDir * (remainingMoveDistance * 0.9f);
                    HandleCollisions(ref targetColliderPos, ++recursionDepth);
                }
            }
        }

#nullable enable warnings

        private Vector3 GetTransformBasedTargetPosition()
        {
            return transform.position + transform.TransformDirection(PivotPositionOffset + objPosOffset);
        }

        private void ClampPivotPositionBySurface(ref Vector3 targetPos)
        {
            if (ColliderCenter == null)
                return;

            var colliderPos = ColliderCenter.Value;
            var travelDir = targetPos - colliderPos;
            var travelDirNormalized = travelDir.normalized;
            var travelDistance = travelDir.magnitude;

            if (Physics.Raycast(
                ColliderCenter.Value,
                travelDirNormalized,
                out var hit,
                travelDistance * 1.2f,
                SurfaceCastMask,
                QueryTriggerInteraction.Ignore
                ))
            {
                if (hit.distance < 0.01f)
                    return;

                targetPos = colliderPos + Vector3.ClampMagnitude(travelDir, hit.distance);
            }
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
    }
}
