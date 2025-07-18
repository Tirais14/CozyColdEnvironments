using System;

#nullable enable
namespace UTIRLib.InputSystem
{
    [Serializable]
    public struct InputItem
    {
        public string ActionName { get; set; }
        public InputActionValueType ValueType { get; set; }
    }
}
