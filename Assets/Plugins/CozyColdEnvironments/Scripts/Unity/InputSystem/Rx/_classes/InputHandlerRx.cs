using CCEnvs.Conversations;
using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.InputSystem;

#nullable enable
namespace CCEnvs.Unity.InputSystem.Rx
{
    public abstract class InputHandlerRx
        :
        DisposableContainer,
        IInputHandlerRx
    {
        private readonly Dictionary<string, IInputActionRx> registeredActions = new(0);

        public InputActionMap ActionMap { get; }
        public bool IsEnabled { get; private set; }

        protected InputHandlerRx(InputActionMap actionMap, bool autoSetProps)
        {
            ActionMap = actionMap;

            if (autoSetProps)
                SetProperties();

            Enable();
        }

        /// <exception cref="EmptyStringArgumentException"></exception>
        public IInputActionRx GetInputAction(string inputName)
        {
            if (inputName.IsNullOrEmpty())
                throw new EmptyStringArgumentException(nameof(inputName), inputName);
            if (!registeredActions.TryGetValue(inputName, out IInputActionRx result))
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
                CCDebug.PrintLog("Enabled", new DebugContext(GetType()));
            }
            catch (Exception ex)
            {
                CCDebug.PrintException(ex);
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

                CCDebug.PrintLog("Disabled", new DebugContext(GetType()));
            }
            catch (Exception ex)
            {
                CCDebug.PrintException(ex);
                IsEnabled = false;
            }
        }

        protected void RegsiterAction(IInputActionRx inputAction)
        {
            if (inputAction.IsNull())
                throw new ArgumentNullException(nameof(inputAction));

            registeredActions.Add(inputAction.ActionName, inputAction);
            disposables.Add(inputAction);
        }

        private static Type? ResolveValueType(PropertyInfo prop)
        {
            return (Type?)TypeHelper.CollectBaseTypes(prop.PropertyType)
                                    .FirstOrDefault(type => type.IsGenericType)
                                    .Maybe()
                                    .Map(type => type.GetGenericArguments()[0]);
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
                         .Where(x => x.PropertyType.IsType<IInputActionRx>()).ToArray();

            if (props.IsNullOrEmpty())
            {
                CCDebug.PrintWarning("Cannot find any input action properties.");
                return;
            }

            registeredActions.EnsureCapacity(props.Length);

            IInputActionRx action;
            foreach (var prop in props)
            {
                action = InputActionRxFactory.Create(ResolveValueType(prop), ResolveInputAction(prop));
                prop.SetValue(this, action);
                RegsiterAction(action);
            }

            registeredActions.TrimExcess();
            CCDebug.PrintLog("Initialized", new DebugContext(GetType()).Additive());
        }
    }
}

