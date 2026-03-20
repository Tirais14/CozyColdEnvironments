using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.InputSystem.Rx;
using R3;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{
    [DisallowMultipleComponent]
    public class CharControllerSimplenputBinder : CCBehaviour
    {
        [SerializeField]
        protected InputActionRxReference<Vector2> moveInputAction;

        [SerializeField]
        protected ButtonActionRxReference jumpInputAction;

        [GetBySelf]
        private CharControllerSimple charController = null!;

        private IDisposable? moveIABinding;
        private IDisposable? jumpIABinding;

        public InputActionRx<Vector2>? MoveIA { get; private set; }

        public ButtonActionRx? JumpIA { get; private set; }

        protected override void Start()
        {
            base.Start();

            if (MoveIA == null && moveInputAction != null && moveInputAction.IsValid)
            {
                MoveIA = moveInputAction.Value;
            }

            if (JumpIA == null && jumpInputAction != null && jumpInputAction.IsValid)
            {
                JumpIA = jumpInputAction.Value;
                TryBindJumpIA();
            }
        }

        protected virtual void Update()
        {
            if (MoveIA != null)
                OnMoveIA(MoveIA.ReadValue());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            moveIABinding?.Dispose();
            jumpIABinding?.Dispose();
        }

        public CharControllerSimplenputBinder SetMoveIA(InputActionRx<Vector2>? ia)
        {
            MoveIA = ia;
            return this;
        }

        public CharControllerSimplenputBinder SetJumpIA(ButtonActionRx? ia)
        {
            JumpIA = ia;
            TryBindJumpIA();
            return this;
        }

        private void TryBindJumpIA()
        {
            jumpIABinding?.Dispose();
            jumpIABinding = null;

            if (JumpIA == null)
                return;

            jumpIABinding = JumpIA.ObservePerformed()
                .Subscribe(this,
                static (_, @this) =>
                {
                    @this.charController.Jump();
                });
        }

        private void OnMoveIA(Vector2 inputValue)
        {
            charController.MoveByInput(inputValue);
        }
    }
}
