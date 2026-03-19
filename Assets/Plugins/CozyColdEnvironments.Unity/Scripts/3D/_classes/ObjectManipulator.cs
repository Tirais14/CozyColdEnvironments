using CCEnvs.Unity.Components;
using CCEnvs.Unity.EditorSerialization;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{
    public sealed class ObjectManipulator : CCBehaviour
    {
        public const float ITEM_MOVE_SENSIVITY_MIN = 0f;
        public const float ITEM_MOVE_DAMPING_MIN = 0f;
        public const float ITEM_ROTATION_SENSIVITY_MIN = 0f;
        public const float ITEM_ROTATION_DAMPING_MIN = 0f;

        [SerializeField]
        private Transform itemAnchor;

        [Header("Behaviour Settings")]

        [SerializeField]
        private CommonSettings settings = CommonSettings.Default;

        [SerializeField]
        private OffsetSettings offsetSettings = OffsetSettings.Default;

        [SerializeField]
        private VelocitySettings velocitySettings = VelocitySettings.Default;

        [Header("Surface Colliding Settings")]
        [Space(6f)]

        [SerializeField]
        private SerializedNullable<LayerMask> surfaceCastMask;

        [Header("Move Settings")]
        [Space(6f)]

        [SerializeField, Min(ITEM_MOVE_SENSIVITY_MIN)]
        private float itemMoveSensivity = 1f;

        [SerializeField, Min(ITEM_MOVE_DAMPING_MIN)]
        private float itemMoveDamping = 15f;

        [SerializeField, Min(ITEM_ROTATION_SENSIVITY_MIN)]
        private float itemRotationSensivity = 1f;

        [SerializeField, Min(ITEM_ROTATION_DAMPING_MIN)]
        private float itemRotationDamping = 15f;

        private Vector3 itemPos;
        private Vector3 itemPosOffset;

        private Quaternion itemRot;
        private Quaternion itemRotOffset;

        private float? objectRadius;

        public Rigidbody? Object { get; private set; }

        public Collider? ObjectCollider { get; private set; }

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

            Gizmos.DrawWireSphere(Object.position, ResolveObjectRadius());
        }
#endif

        public ObjectManipulator ApplyTranslation(Vector3 deltaPos)
        {
            itemPosOffset += deltaPos * itemMoveSensivity;

            return this;
        }

        public ObjectManipulator ApplyRotation(Vector3 deltaEuler)
        {
            var rotation = deltaEuler * itemRotationSensivity;

            itemRotOffset *= Quaternion.Euler(rotation);

            return this;
        }

        public ObjectManipulator SetPositionOffset(Vector3 posOffset)
        {
            itemPosOffset = posOffset;

            return this;
        }

        public ObjectManipulator SetRotationOffset(Quaternion rotOffset)
        {
            itemRotOffset = rotOffset;

            return this;
        }

        public ObjectManipulator ResetPositionOffset()
        {
            itemPosOffset = Vector3.zero;

            return this;
        }

        public ObjectManipulator ResetRotationOffset()
        {
            itemRotOffset = Quaternion.identity;

            return this;
        }

        public ObjectManipulator SetObject(Rigidbody? value)
        {
            if (value == null && Object != null)
            {
                OnDropObject();
                Object = null!;
            }
            else if (value != null)
            {

                Object = value;
                OnSetObject();
            }

            return this;
        }

#nullable disable warnings
        private void OnDropObject()
        {
            Object.useGravity = true;
            Object.isKinematic = false;

            if (velocitySettings.IsFlagSetted(VelocitySettings.ResetOnDrop))
            {
                Object.linearVelocity = Vector3.zero;
                Object.angularVelocity = Vector3.zero;
            }

            objectRadius = null;
        }

        private void InObjectBounds()
        {
            //colliderBounds
        }

        private float ResolveObjectRadius()
        {
            if (objectRadius != null
                &&
                settings.IsFlagSetted(CommonSettings.ObjectSizeChangeable))
            {
                return objectRadius.Value;
            }

            var extents = ObjectCollider.bounds.extents;

            float bigger = 0f;

            float value;

            for (int i = 0; i < 3; i++)
            {
                value = extents[i];

                if (value > bigger)
                    bigger = value;
            }

            objectRadius = bigger;

            return objectRadius.Value;
        }

        private void SetObjectCollider()
        {
            ObjectCollider = Object.GetComponent<Collider>();
            ResolveObjectRadius();
        }

        private void OnSetObject()
        {
            itemPos = Object.transform.position;
            itemRot = Object.transform.rotation;

            ResetPositionOffset().ResetRotationOffset();

            Object.useGravity = false;
            Object.isKinematic = true;

            if (velocitySettings.IsFlagSetted(VelocitySettings.ResetOnSet))
            {
                Object.linearVelocity = Vector3.zero;
                Object.angularVelocity = Vector3.zero;
            }

            SetObjectCollider();
        }

        private void HandleCollisions(ref Vector3 targetPos)
        {
            var moveDir = targetPos - Object.position;
            var moveDirNormalized = moveDir.normalized;
            var moveDistance = moveDir.magnitude;

            if (moveDistance < 0.001f)
                return;

            var objRadius = ResolveObjectRadius();
            float castDistance = moveDistance + objRadius * 0.1f;

            if (Physics.SphereCast(
                Object.position,
                ResolveObjectRadius(),
                moveDirNormalized,
                out var hit,
                castDistance,
                surfaceCastMask.Deserialized ?? Physics.AllLayers,
                QueryTriggerInteraction.Ignore
                ))
            {
                targetPos = hit.point + hit.normal * (objRadius * 1.02f);

                float traveled = hit.distance;
                float remaining = moveDistance - traveled;

                if (remaining > 0.01f)
                {
                    Vector3 slideDir = Vector3.ProjectOnPlane(moveDirNormalized, hit.normal);
                    targetPos += slideDir * (remaining * 0.9f);
                }
            }
        }

#nullable enable warnings

        private void HandleObjectTransform()
        {
            if (Object == null)
                return;

            Vector3 targetPos = itemAnchor.position + itemAnchor.TransformDirection(itemPosOffset);
            Quaternion targetRot = itemAnchor.rotation * itemRotOffset;

            if (settings.IsFlagSetted(CommonSettings.CollideWithSurface))
                HandleCollisions(ref targetPos);

            float moveT = 1f - Mathf.Exp(-itemMoveDamping * Time.fixedDeltaTime);
            float rotT = 1f - Mathf.Exp(-itemRotationDamping * Time.fixedDeltaTime);

            itemPos = Vector3.Lerp(itemPos, targetPos, moveT);
            itemRot = Quaternion.Slerp(itemRot, targetRot, rotT);

            Object.Move(itemPos, itemRot);
        }

        [Flags]
        public enum CommonSettings
        {
            None,
            CollideWithSurface,
            ObjectSizeChangeable,
            Default = CollideWithSurface
        }

        [Flags]
        public enum OffsetSettings
        {
            None,
            ResetOnSet,
            ResetOnDrop,
            Default = None
        }

        [Flags]
        public enum VelocitySettings
        {
            None,
            ResetOnSet,
            ResetOnDrop,
            Default = ResetOnSet | ResetOnDrop
        }
    }
}
