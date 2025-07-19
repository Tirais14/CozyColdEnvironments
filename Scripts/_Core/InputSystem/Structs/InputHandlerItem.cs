using System;
using UnityEngine;

#nullable enable
namespace UTIRLib.InputSystem
{
    [Serializable]
    public struct InputHandlerItem
    {
        [field: SerializeField]
        public string ActionName { get; private set; }

        [field: SerializeField]
        public InputActionValueType ValueType { get; private set; }
    }
}
