using UnityEngine;

#nullable enable
namespace UTIRLib
{
    [RequireComponent(typeof(Camera))]
    public class FirstPersonCameraController : MonoX
    {
        private float cameraSensivity = 21f;
        private float verticalAngle = 0f;
        private float horizontalAngle = 0f;

        public float CameraSensivity {
            get => cameraSensivity;
            set
            {
                if (value < 0f)
                    value = 0f;

                cameraSensivity = value;
            }
        }

        public void Rotate(Vector3 direction)
        {
            float inputX = direction.x * Time.deltaTime * cameraSensivity;
            float inputY = direction.y * Time.deltaTime * cameraSensivity;

            verticalAngle -= inputY;
            verticalAngle = Mathf.Clamp(verticalAngle, -90f, 90f);

            horizontalAngle += inputX;

            transform.localRotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);
        }
    }
}
