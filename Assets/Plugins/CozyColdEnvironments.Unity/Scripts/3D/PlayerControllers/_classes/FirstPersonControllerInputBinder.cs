using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.InputSystem.Rx;
using R3;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3.Controllers
{
    [DisallowMultipleComponent]
    public sealed class FirstPersonControllerInputBinder : CCBehaviour
    {
        [SerializeField]
        private InputActionRxReference<Vector2> lookInputAction;

        [GetBySelf]
        private FirstPersonController firstPersonController = null!;

        private IDisposable? lookIABinding;

        public InputActionRx<Vector2>? LookIA { get; private set; }

        protected override void Start()
        {
            base.Start();

            if (LookIA == null && lookInputAction != null && lookInputAction.IsValid)
            {
                LookIA = lookInputAction.Value;
                TryBindLookIA();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            lookIABinding?.Dispose();
        }

        public FirstPersonControllerInputBinder SetLookIA(InputActionRx<Vector2>? ia)
        {
            LookIA = ia;
            TryBindLookIA();
            return this;
        }

        private void TryBindLookIA()
        {
            lookIABinding?.Dispose();
            lookIABinding = null;

            if (LookIA == null)
                return;

            lookIABinding = LookIA.ObservePerformedValue()
                .Subscribe(this,
                static (inputValue, @this) =>
                {
                    @this.firstPersonController.MoveCamera(inputValue);
                });
        }
    }
}