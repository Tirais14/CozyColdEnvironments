using CCEnvs.UnityX.ComponentInjections;
using CCEnvs.UnityX.Components;
using CCEnvs.UnityX.D3.Controllers;
using CCEnvs.UnityX.InputSystem.Rx;
using R3;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3.Controllers
{
    public abstract class BasePersonControllerInputBinder : CCBehaviour
    {
        [SerializeField]
        protected InputActionRxReference<Vector2> lookInputAction;

        [GetBySelf]
        protected BasePersonController firstPersonController = null!;

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

        public void SetLookIA(InputActionRx<Vector2>? ia)
        {
            LookIA = ia;
            TryBindLookIA();
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
