using CCEnvs.Dependencies;
using CCEnvs.Unity.InputSystem.Rx;
using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable
namespace Core.InputSystem
{
    public class PlayerInputHandler : InputHandlerRx
    {
        public InputActionRx<Vector2> Move { get; private set; } = null!;

        public ButtonActionRx Jump { get; private set; } = null!;

        public InputActionRx<Vector2> Look { get; private set; } = null!;

        public PlayerInputHandler(InputActionMap actionMap) 
            :
            base(
                actionMap,
                autoSetProps: true
                )
        {
            CCServices.Bind(Move, CCServices.MOVE_INPUT_ACTION_CONTAINER_KEY);
            CCServices.Bind(Jump, CCServices.JUMP_INPUT_ACTION_CONTAINER_KEY);
            CCServices.Bind(Look, CCServices.LOOK_INPUT_ACTION_CONTAINER_KEY);
        }
    }
}
