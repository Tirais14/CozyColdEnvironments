using CCEnvs.Diagnostics;
using CCEnvs.Unity.EditorSerialization;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable
namespace CCEnvs.Unity.InputSystem.Reactive
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
