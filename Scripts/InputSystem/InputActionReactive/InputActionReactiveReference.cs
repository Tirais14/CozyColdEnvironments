using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UTIRLib.Diagnostics;
using UTIRLib.Unity.Serialization;

#nullable enable
namespace UTIRLib.InputSystem.Reactive
{
    [Serializable]
    public sealed class InputActionReactiveReference
    {
        private IInputActionReactive action = null!;

        [SerializeField]
        private InputActionReference inputAction = null!;
        [SerializeField]
        private SerializedType inputValueType;

        public IInputActionReactive Action {
            get
            {
                if (action.IsNull())
                    action = InputActionReactiveFactory.Create((Type?)inputValueType, inputAction);

                return action;
            }
        } 
    }
}
