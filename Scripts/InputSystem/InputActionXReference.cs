using System;
using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable
namespace UTIRLib.InputSystem
{
    [Serializable]
    public class InputActionXReference
    {
        private IInputAction? action;

        [SerializeField]
        protected InputActionReference actionReference;

        public IInputAction Action {
            get
            {
                action ??= new InputActionX(actionReference);

                return action;
            }
        }

        public InputActionXReference(InputActionReference actionReference)
        {
            this.actionReference = actionReference;
        }
    }

    [Serializable]
    public class InputActionXReference<T>
        where T : struct
    {
        private IInputAction<T>? action;

        [SerializeField]
        protected InputActionReference actionReference;

        public IInputAction<T> Action {
            get
            {
                action ??= InputActionFactory.Create<T>(actionReference);

                return action;
            }
        }

        public InputActionXReference(InputActionReference actionReference)
        {
            this.actionReference = actionReference;
        }
    }
}
