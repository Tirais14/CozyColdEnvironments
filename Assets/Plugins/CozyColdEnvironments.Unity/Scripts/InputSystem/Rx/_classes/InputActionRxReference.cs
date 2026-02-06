using CCEnvs.Diagnostics;
using CCEnvs.Unity.Serialization;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable
namespace CCEnvs.Unity.InputSystem.Rx
{
    [Serializable]
    public sealed class InputActionRxReference
    {
        private IInputActionRx action = null!;

        [SerializeField]
        private InputActionReference inputAction = null!;
        [SerializeField]
        private SerializedType inputValueType;

        public IInputActionRx Action {
            get
            {
                if (action.IsNull())
                    action = InputActionRxFactory.Create((Type?)inputValueType, inputAction);

                return action;
            }
        } 
    }
}
