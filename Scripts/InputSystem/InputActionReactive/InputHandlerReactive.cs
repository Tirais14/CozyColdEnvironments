using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.InputSystem;
using UTIRLib.Diagnostics;
using UTIRLib.Disposables;
using UTIRLib.Reflection;

#nullable enable
namespace UTIRLib.InputSystem.Reactive
{
    public abstract class InputHandlerReactive
        :
        DisposableContainer,
        IInputHandlerReactive
    {
        private readonly Dictionary<string, IInputActionReactive> registeredActions = new(0);

        public InputActionMap ActionMap { get; }
        public bool IsEnabled { get; private set; }

        protected InputHandlerReactive(InputActionMap actionMap, bool autoSetProps)
        {
            ActionMap = actionMap;

            if (autoSetProps)
                SetProperties();
        }

        /// <exception cref="StringArgumentException"></exception>
        public IInputActionReactive GetInputAction(string inputName)
        {
            if (inputName.IsNullOrEmpty())
                throw new StringArgumentException(nameof(inputName), inputName);
            if (!registeredActions.TryGetValue(inputName, out IInputActionReactive result))
                throw new ArgumentException($"Cannot find input action with name {inputName}.");

            return result;
        }

        public void Enable()
        {
            try
            {
                foreach (var item in registeredActions.Values)
                    item.Enable();

                IsEnabled = true;
            }
            catch (Exception ex)
            {
                TirLibDebug.PrintException(ex);
                IsEnabled = false;
            }
        }

        public void Disable()
        {
            try
            {
                foreach (var item in registeredActions.Values)
                    item.Disable();

                IsEnabled = false;
            }
            catch (Exception ex)
            {
                TirLibDebug.PrintException(ex);
                IsEnabled = false;
            }
        }

        protected void RegsiterAction(IInputActionReactive inputAction)
        {
            if (inputAction.IsNull())
                throw new ArgumentNullException(nameof(inputAction));

            registeredActions.Add(inputAction.ActionName, inputAction);
            disposables.Add(inputAction);
        }

        private static Type? ResolveValueType(PropertyInfo prop)
        {
            if (!prop.PropertyType.IsGenericType)
                return null;

            return prop.PropertyType.GetGenericArguments()[0];
        }

        private InputAction ResolveInputAction(PropertyInfo prop)
        {
            InputAction resolved = ActionMap.FindAction(prop.Name, throwIfNotFound: true);

            return resolved;
        }

        private void SetProperties()
        {
            PropertyInfo[] props =
                GetType().ForceGetProperties(BindingFlagsDefault.InstancePublic)
                         .Where(x => x.PropertyType.IsType<IInputActionReactive>()).ToArray();

            if (props.IsNullOrEmpty())
            {
                TirLibDebug.PrintWarning("Cannot find any input action properties.");
                return;
            }

            registeredActions.EnsureCapacity(props.Length);

            IInputActionReactive action;
            foreach (var prop in props)
            {
                action = InputActionReactiveFactory.Create(ResolveValueType(prop),
                                                           ResolveInputAction(prop));
                prop.SetValue(this, action);
                RegsiterAction(action);
            }

            registeredActions.TrimExcess();
        }
    }
}

