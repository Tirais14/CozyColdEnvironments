#nullable enable
using CCEnvs.Unity.EditorSerialization;
using System;
using UnityEngine;
using Options = CCEnvs.Unity.D3.ObjectManipulatorSettings;
using OffsetOptions = CCEnvs.Unity.D3.ObjectManipulatorOffsetSettings;
using VelocityOptions = CCEnvs.Unity.D3.ObjectManipulatorVelocitySettings;
using CommunityToolkit.Diagnostics;

namespace CCEnvs.Unity.D3
{
    public sealed partial class ObjectManipulator
    {
        public ObjectManipulator Configure(Action<ObjectManipulator> configurer)
        {
            Guard.IsNotNull(configurer, nameof(configurer));

            configurer(this);

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

        public ObjectManipulator SetCollisionDetectionEveryFrame(long value)
        {
            collisionDetectionEveryFrame = Math.Max(value, COLLISTION_DETECTION_EVERY_FRAME_MIN);
            return this;
        }

        public ObjectManipulator SetManipulatorObjectPitchOffset(float value)
        {
            manipulatorObjectPitchOffset = Mathf.Max(value, MANIPULATOR_OBJECT_PITCH_OFFSET_MIN);
            maniputalorObjRotOffset = null;
            return this;
        }

        public ObjectManipulator SetManipulatorObjectYawOffset(float value)
        {
            manipulatorObjectYawOffset = Mathf.Max(value, MANIPULATOR_OBJECT_YAW_OFFSET_MIN);
            maniputalorObjRotOffset = null;
            return this;
        }

        public ObjectManipulator SetManipulatorObjectRollOffset(float value)
        {
            manipulatorObjectRollOffset = Mathf.Max(value, MANIPULATOR_OBJECT_ROLL_OFFSET_MIN);
            maniputalorObjRotOffset = null;
            return this;
        }

        public ObjectManipulator SetManipulatorObjectDistanceOffset(float value)
        {
            manipulatorObjectDistanceOffset = Mathf.Max(value, MANIPULATOR_OBJECT_DISTANCE_OFFSET_MIN);
            maniputalorObjPosOffset = null;
            return this;
        }

        public ObjectManipulator SetManipulatorObjectHorizontalOffset(float value)
        {
            manipulatorObjectHorizontalOffset = Mathf.Max(value, MANIPULATOR_OBJECT_HORIZONTAL_OFFSET_MIN);
            maniputalorObjPosOffset = null;
            return this;
        }

        public ObjectManipulator SetManipulatorObjectVerticalOffset(float value)
        {
            manipulatorObjectVerticalOffset = Mathf.Max(value, MANIPULATOR_OBJECT_VERTICAL_OFFSET_MIN);
            maniputalorObjPosOffset = null;
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
            objectMoveSensivity = Mathf.Max(value, OBJECT_MOVE_SENSIVITY_MIN);
            return this;
        }

        public ObjectManipulator SetMoveDamping(float value)
        {
            objectMoveDamping = Mathf.Max(value, OBJECT_MOVE_DAMPING_MIN);
            return this;
        }

        public ObjectManipulator SetRotationSensitivity(float value)
        {
            objectRotationSensivity = Mathf.Max(value, OBJECT_ROTATION_SENSIVITY_MIN);
            return this;
        }

        public ObjectManipulator SetRotationDamping(float value)
        {
            objectRotationDamping = Mathf.Max(value, OBJECT_ROTATION_DAMPING_MIN);
            return this;
        }

        public ObjectManipulator SetSurfaceCastMask(LayerMask? mask)
        {
            surfaceCastMask = new SerializedNullable<LayerMask>(mask);
            return this;
        }

        public ObjectManipulator SetSettings(Options value)
        {
            settings = value;
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
            objPosOffset += deltaPos * objectMoveSensivity;

            return this;
        }

        public ObjectManipulator ApplyRotation(Vector3 deltaEuler)
        {
            var rotation = deltaEuler * objectRotationSensivity;

            objRotOffset *= Quaternion.Euler(rotation);

            return this;
        }
    }
}
