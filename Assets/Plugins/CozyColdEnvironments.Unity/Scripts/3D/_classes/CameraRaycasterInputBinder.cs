using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.InputSystem.Rx;
using R3;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{
    [RequireComponent(typeof(ICameraRaycaster))]
    public class CameraRaycasterInputBinder : CCBehaviour
    {
        [Header("Raycast Input Actions")]
        [Space(6f)]

        [SerializeField]
        private InputActionRxReference<Vector2> raycastInputAction;

        [GetBySelf]
        private CameraRaycasterRay raycaster = null!;

        private IDisposable? raycastIABinding;

        public InputActionRx<Vector2>? RaycastIA { get; private set; }

        protected override void Start()
        {
            base.Start();

            if (RaycastIA == null && raycastInputAction != null && raycastInputAction.IsValid)
            {
                RaycastIA = raycastInputAction.Value;
                TryBindRaycastIA();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            raycastIABinding?.Dispose();
        }

        public CameraRaycasterInputBinder SetRaycastIA(InputActionRx<Vector2>? ia)
        {
            RaycastIA = ia;
            TryBindRaycastIA();
            return this;
        }

        private void TryBindRaycastIA()
        {
            raycastIABinding?.Dispose();
            raycastIABinding = null;

            if (RaycastIA == null)
                return;

            raycastIABinding = RaycastIA.ObservePerformedValue()
                .Subscribe(OnRaycastIA);
        }

        private void OnRaycastIA(Vector2 inputValue)
        {
            raycaster.TryRaycast(inputValue);
        }
    }
}
