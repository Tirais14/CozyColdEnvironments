using CCEnvs.Attributes;
using CCEnvs.Disposables;
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
        private ButtonActionRxReference raycastInputAction;

        [SerializeField, OptionalField]
        private InputActionRxReference<Vector2> pointInputAction;

        [GetBySelf]
        private ICameraRaycaster raycaster = null!;

        private IDisposable? raycastIABinding;

        public InputActionRx<Vector2>? PointIA { get; private set; }

        public ButtonActionRx? RaycastIA { get; private set; }

        protected override void Start()
        {
            base.Start();

            if (PointIA == null && pointInputAction != null && pointInputAction.IsValid)
            {
                PointIA = pointInputAction.Value;
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
            CCDisposable.Dispose(ref raycastIABinding);
        }

        public CameraRaycasterInputBinder SetPointIA(InputActionRx<Vector2>? ia)
        {
            PointIA = ia;
            return this;
        }

        public CameraRaycasterInputBinder SetRaycastIA(ButtonActionRx? ia)
        {
            RaycastIA = ia;
            TryBindRaycastIA();
            return this;
        }

        private void TryBindRaycastIA()
        {
            CCDisposable.Dispose(ref raycastIABinding);

            if (RaycastIA == null)
                return;

            raycastIABinding = RaycastIA.ObservePerformed()
                .Subscribe(OnRaycastIA);
        }

        private void OnRaycastIA(InputAction.CallbackContext _)
        {
            if (TryGetPointValue(out var screePoint))
                raycaster.TryRaycast(screePoint);
            else
                raycaster.TryRaycast();
        }

        private bool TryGetPointValue(out Vector2 screePoint)
        {
            if (PointIA == null)
            {
                screePoint = Vector2.zero;  
                return false;
            }

            screePoint = PointIA.ReadValue();
            return true;
        }
    }
}