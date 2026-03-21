using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3.Controllers
{
    public class FirstPersonController : CharController
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

        public FirstPersonController SetCameraProxy(Transform value)
        {
            CC.Guard.IsNotNull(value);

            cameraProxy = value;

            return this;
        }

        public FirstPersonController SetHorizontalCameraSensivity(float value)
        {
            horizontalCameraSensivity = Mathf.Max(CAMERA_SENSIVITY_MIN, value);

            return this;
        }

        public FirstPersonController SetVerticalCameraSensivity(float value)
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