using CCEnvs.Unity.D3.Controllers;
using CCEnvs.Unity.InputSystem.Rx;
using Core.InputSystem;
using R3;
using System;
using UnityEngine;
using Zenject;

#nullable enable
namespace Core.Playables
{
    public class CharacterController : CharControllerSimple
    {
        private PlayerInputHandler playerInputHandler = null!;

        private InputActionRx<Vector2> moveInput => playerInputHandler.Move;

        private IDisposable? jumpBinding;

        private ButtonActionRx jumpInput => playerInputHandler.Jump;

        protected override void Awake()
        {
            base.Awake();
        }

        [Inject]
        private void Construct(PlayerInputHandler playerInputHandler)
        {
            this.playerInputHandler = playerInputHandler;

            BindJumpInput();
        }

        protected override void Update()
        {
            base.Update();

            if (moveInput.Action.IsPressed())
                MoveByInput(moveInput.Action.ReadValue<Vector2>());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            jumpBinding?.Dispose();
        }

        private void BindJumpInput()
        {
            jumpBinding = jumpInput.ObservePerformed()
                .Subscribe(this,
                static (_, @this) =>
                {
                    @this.Jump();
                });
        }
    }
}
