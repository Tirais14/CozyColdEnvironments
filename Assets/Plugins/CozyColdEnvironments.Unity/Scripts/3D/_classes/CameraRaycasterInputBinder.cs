using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.InputSystem.Rx;
using R3;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable
namespace CCEnvs.Unity.D3
{
    [RequireComponent(typeof(ICameraRaycaster))]
    public class CameraRaycasterInputBinder : CCBehaviour
    {
        [Header("Raycast Input Actions")]
        [Space(6f)]

        [SerializeField]
        private InputActionRxReference<Vector2> pointInputAction;

        [SerializeField]
        private ButtonActionRxReference raycastInputAction;

        [GetBySelf]
        private ICameraRaycaster raycaster = null!;

        private IDisposable? pointIABinding;
        private IDisposable? raycastIABinding;

        private Vector2? currentPoint;

        public InputActionRx<Vector2>? PointIA { get; private set; }
        public ButtonActionRx? RaycastIA { get; private set; }

        protected override void Start()
        {
            base.Start();

            if (PointIA == null && pointInputAction != null && pointInputAction.IsValid)
            {
                PointIA = pointInputAction.Value;
                TryBindPointIA();
            }

            if (RaycastIA == null && raycastInputAction != null && raycastInputAction.IsValid)
            {
                RaycastIA = raycastInputAction.Value;
                TryBindRaycastIA();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            pointIABinding?.Dispose();
            raycastIABinding?.Dispose();
        }

        public CameraRaycasterInputBinder SetPointIA(InputActionRx<Vector2>? ia)
        {
            PointIA = ia;
            TryBindPointIA();
            return this;
        }

        public CameraRaycasterInputBinder SetRaycastIA(ButtonActionRx? ia)
        {
            RaycastIA = ia;
            TryBindRaycastIA();
            return this;
        }

        private void TryBindPointIA()
        {
            pointIABinding?.Dispose();
            pointIABinding = null;

            if (PointIA == null)
                return;

            pointIABinding = PointIA.ObservePerformedValue()
                .Subscribe(OnPointIA);
        }

        private void TryBindRaycastIA()
        {
            raycastIABinding?.Dispose();
            raycastIABinding = null;

            if (RaycastIA == null)
                return;

            raycastIABinding = RaycastIA.ObservePerformed()
                .Subscribe(OnRaycastIA);
        }

        private void OnPointIA(Vector2 inputValue)
        {
            currentPoint = inputValue;
        }

        private void OnRaycastIA(InputAction.CallbackContext _)
        {
            if (currentPoint.HasValue)
                raycaster.TryRaycast(currentPoint.Value);
        }
    }
}