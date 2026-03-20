using CCEnvs.Dependencies;
using CCEnvs.Disposables;
using CCEnvs.Unity.InputSystem.Rx;
using CommunityToolkit.Diagnostics;
using R3;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3.Controllers
{
    public class FirstPersonControllerSimple : CharControllerSimple
    {
        public const float CAMERA_SENSIVITY_MIN = 0f;

        [Header("Camera")]
        [Space(6f)]

        [SerializeField]
        protected Transform cameraProxy = null!;

        [SerializeField, Min(CAMERA_SENSIVITY_MIN)]
        protected float horizontalCameraSensivity = 13f;

        [SerializeField, Min(CAMERA_SENSIVITY_MIN)]
        protected float verticalCameraSensivity = 13f;

        private float xRotation;

        public Transform CameraProxy => cameraProxy;

        public float HorizontalCameraSensivity => horizontalCameraSensivity;
        public float VerticalCameraSensivity => verticalCameraSensivity;

        protected override void Start()
        {
            base.Start();

            if (cameraProxy == null)
                throw new MissingComponentException(nameof(cameraProxy));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public FirstPersonControllerSimple SetCameraProxy(Transform value)
        {
            CC.Guard.IsNotNull(value);

            cameraProxy = value;

            return this;
        }

        public FirstPersonControllerSimple SetHorizontalCameraSensivity(float value)
        {
            horizontalCameraSensivity = Mathf.Max(CAMERA_SENSIVITY_MIN, value);

            return this;
        }

        public FirstPersonControllerSimple SetVerticalCameraSensivity(float value)
        {
            verticalCameraSensivity = Mathf.Max(CAMERA_SENSIVITY_MIN, value);

            return this;
        }

        public void MoveCamera(Vector2 moveInput)
        {
            if (moveInput == Vector2.zero)
                return;

            moveInput.x *= horizontalCameraSensivity * Time.deltaTime;
            moveInput.y *= verticalCameraSensivity * Time.deltaTime;

            xRotation -= moveInput.y;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            cameraProxy.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            transform.Rotate(Vector3.up * moveInput.x);
        }
    }
}