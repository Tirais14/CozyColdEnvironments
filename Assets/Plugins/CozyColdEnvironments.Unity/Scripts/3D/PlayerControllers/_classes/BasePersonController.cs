using CCEnvs.UnityX.Components;
using UnityEngine;

namespace CCEnvs.Unity.D3
{
    public abstract class BasePersonController : CCBehaviour
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

        public float HorizontalCameraSensivity {
            get => horizontalCameraSensivity;
            set => horizontalCameraSensivity = Mathf.Max(CAMERA_SENSIVITY_MIN, value);
        }
        public float VerticalCameraSensivity {
            get => verticalCameraSensivity;
            set => verticalCameraSensivity = Mathf.Max(CAMERA_SENSIVITY_MIN, value);
        }

        protected override void Start()
        {
            base.Start();

            if (cameraProxy == null)
                throw new MissingComponentException(nameof(cameraProxy));
        }

        public void SetCameraProxy(Transform value)
        {
            CC.Guard.IsNotNull(value);

            cameraProxy = value;
        }

        public void MoveCamera(Vector2 moveInput)
        {
            if (moveInput == Vector2.zero)
                return;

            moveInput.x *= horizontalCameraSensivity * Mathf.Min(Time.deltaTime, 0.1f);
            moveInput.y *= verticalCameraSensivity * Mathf.Min(Time.deltaTime, 0.1f);

            xRotation -= moveInput.y;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            cameraProxy.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            transform.Rotate(Vector3.up * moveInput.x);
        }
    }
}
