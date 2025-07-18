#nullable enable
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UTIRLib.Init;

namespace UTIRLib.InputSystem
{
    [Obsolete]
    public class InputHandler : MonoXInitable, IInputHandler, IInitable
    {
        [SerializeField]
        private InputActionAsset _inputs;

        [SerializeField]
        private InputItem[] _inputItems;

        [SerializeField]
        private string _mapName;

        protected override void OnInit()
        {
            InputActionMap actionMap = _inputs.FindActionMap(_mapName, throwIfNotFound: true);


        }
    }
}
