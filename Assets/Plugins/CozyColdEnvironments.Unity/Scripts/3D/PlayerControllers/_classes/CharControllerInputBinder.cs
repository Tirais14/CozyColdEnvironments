using CCEnvs.Disposables;
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
    public class CharControllerInputBinder : CCBehaviour
    {
        [Header("Input Actions")]
        [Space(6f)]

        [SerializeField]
        protected InputActionRxReference<Vector2> moveInputAction;

        [SerializeField]
        protected ButtonActionRxReference jumpInputAction;

        [Space(6f)]

        [SerializeField]
        protected bool runToggle = false;

        [SerializeField]
        protected ButtonActionRxReference runInputAction;

        [GetBySelf]
        private CharController charController = null!;

        private IDisposable? jumpIABinding;
        private IDisposable? runIABinding;

        private bool isRunPerformed;
        private bool isRunning;

        public InputActionRx<Vector2>? MoveIA { get; private set; }

        public ButtonActionRx? JumpIA { get; private set; }
        public ButtonActionRx? RunIA { get; private set; }

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

            if (RunIA == null && runInputAction != null && runInputAction.IsValid)
            {
                RunIA = runInputAction.Value;
                TryBindRunIA();
            }
        }

        protected virtual void Update()
        {
            if (MoveIA != null)
                OnMoveIA(MoveIA.ReadValue());

            ProcessRunInput();

            if (isRunning)
                OnRunIA();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CCDisposable.Dispose(ref jumpIABinding);
            CCDisposable.Dispose(ref runIABinding);
        }

        public CharControllerInputBinder SetMoveIA(InputActionRx<Vector2>? ia)
        {
            MoveIA = ia;
            return this;
        }

        public CharControllerInputBinder SetJumpIA(ButtonActionRx? ia)
        {
            JumpIA = ia;
            TryBindJumpIA();
            return this;
        }

        public CharControllerInputBinder SetRunIA(ButtonActionRx? value)
        {
            RunIA = value;
            TryBindRunIA();
            return this;
        }

        private void TryBindJumpIA()
        {
            CCDisposable.Dispose(ref jumpIABinding);

            if (JumpIA == null)
                return;

            jumpIABinding = JumpIA.ObservePerformed()
                .Subscribe(this,
                static (_, @this) =>
                {
                    @this.charController.Jump();
                });
        }

        private void TryBindRunIA()
        {
            CCDisposable.Dispose(ref runIABinding);

            if (RunIA == null)
                return;

            var bindingBuilder = Disposable.CreateBuilder();

            var startedBinding = RunIA.ObserveStarted()
                .Subscribe(this,
                static (_, @this) =>
                {
                    @this.isRunPerformed = true;
                });

            var canceledBinding = RunIA.ObserveCanceled()
                .Subscribe(this,
                static (_, @this) =>
                {
                    @this.isRunPerformed = false;
                });

            bindingBuilder.Add(startedBinding);
            bindingBuilder.Add(canceledBinding);

            runIABinding = bindingBuilder.Build();
        }

        private void OnMoveIA(Vector2 inputValue)
        {
            charController.MoveByInput(inputValue);
        }

        private void OnRunIA()
        {
            charController.Run();
        }

        private void ProcessRunInput()
        {
            if (isRunPerformed)
            {
                if (runToggle)
                {
                    isRunning = !isRunning;
                    isRunPerformed = false;
                }
                else
                    isRunning = true;
            }
            else
                isRunning = false;
        }
    }
}
