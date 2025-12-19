using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.InputSystem;

#nullable enable
#pragma warning disable S3881
namespace CCEnvs.Unity.InputSystem.Rx
{
    public abstract class InputHandlerRx
        :
        IInputHandlerRx
    {
        protected readonly List<IDisposable> disposables = new();
        private readonly Dictionary<string, IInputActionRx> registeredActions = new(0);
        private bool disposed;

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
            Guard.IsNotNullOrWhiteSpace(inputName, nameof(inputName));
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
                CCDebug.Instance.PrintLog("Enabled", new DebugContext(GetType()));
            }
            catch (Exception ex)
            {
                CCDebug.Instance.PrintException(ex);
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

                CCDebug.Instance.PrintLog("Disabled", new DebugContext(GetType()));
            }
            catch (Exception ex)
            {
                CCDebug.Instance.PrintException(ex);
                IsEnabled = false;
            }
        }

        public void Dispose() => Dispose(disposing: true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            disposables.DisposeEach();

            disposed = true;
        }

        protected void RegsiterAction(IInputActionRx inputAction)
        {
            if (inputAction.IsNull())
                throw new ArgumentNullException(nameof(inputAction));

            registeredActions.Add(inputAction.ActionName, inputAction);
            disposables.Add(inputAction);
        }

        private InputAction ResolveInputAction(PropertyInfo prop)
        {
            InputAction resolved = ActionMap.FindAction(prop.Name, throwIfNotFound: true);

            return resolved;
        }

        private void SetProperties()
        {
            PropertyInfo[] props = this.Reflect()
                .NonPublic()
                .IncludeBaseTypes()
                .TypeFilter<IInputActionRx>()
                .Properties()
                .Where(x => x.PropertyType.IsType<IInputActionRx>())
                .ToArray();

            if (props.IsNullOrEmpty())
            {
                CCDebug.Instance.PrintError("Cannot find any input action properties.");
                return;
            }

            registeredActions.EnsureCapacity(props.Length);

            IInputActionRx action;
            foreach (var prop in props)
            {
                action = InputActionRxFactory.Create(prop.PropertyType, ResolveInputAction(prop));
                prop.SetValue(this, action);
                RegsiterAction(action);
            }

            registeredActions.TrimExcess();
            CCDebug.Instance.PrintLog("Initialized", new DebugContext(GetType()).Additive());
        }
    }
}

