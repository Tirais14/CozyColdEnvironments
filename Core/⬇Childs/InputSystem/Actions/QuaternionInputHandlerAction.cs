using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable

namespace UTIRLib.InputSystem
{
    public class QuaternionInputHandlerAction : InputHandlerAction<Quaternion>
    {
        public QuaternionInputHandlerAction(InputAction inputAction) : base(inputAction)
        { }
    }
}