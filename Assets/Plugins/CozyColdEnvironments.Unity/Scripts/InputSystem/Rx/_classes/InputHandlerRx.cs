using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CCEnvs.TypeMatching;
using CommunityToolkit.Diagnostics;
using R3;
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
        //#region IL2CPP Defines

        //[Preserve]
        //private static InputActionRx<Vector2>? _vectorActiion;

        //[Preserve]
        //private static InputActionRx<bool>? boolAction;

        //#endregion IL2CPP Defines

        protected readonly List<IDisposable> disposables = new();

        private readonly Dictionary<string, IInputActionRx> registeredActions = new(0);

        private readonly ReactiveProperty<bool> isEnabled;

        public InputActionMap ActionMap { get; }

        public bool IsEnabled => isEnabled.Value && ActionMap.enabled;

        protected InputHandlerRx(InputActionMap actionMap, bool autoSetProps)
        {
            ActionMap = actionMap;
            isEnabled = new ReactiveProperty<bool>(actionMap.enabled);

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

                isEnabled.Value = true;

                if (CCDebug.Instance.IsEnabled)
                    CCDebug.Instance.PrintLog("Enabled", new DebugContext(GetType()));
            }
            catch (Exception ex)
            {
                CCDebug.Instance.PrintException(ex);
                isEnabled.Value = false;
            }
        }

        public void Disable()
        {
            try
            {
                foreach (var item in registeredActions.Values)
                    item.Disable();

                isEnabled.Value = false;

                if (CCDebug.Instance.IsEnabled)
                    CCDebug.Instance.PrintLog("Disabled", new DebugContext(GetType()));
            }
            catch (Exception ex)
            {
                CCDebug.Instance.PrintException(ex);
                isEnabled.Value = false;
            }
        }

        public R3.Observable<bool> ObserveEnabled()
        {
            return isEnabled.Where(static x => x);
        }

        public R3.Observable<bool> ObserveDisabled()
        {
            return isEnabled.Where(static x => !x);
        }

        private bool disposed;
        public void Dispose() => Dispose(disposing: true);
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
                disposables.DisposeEachAndClear();

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
                .IncludeNonPublic()
                .IncludeBaseTypes()
                .WithTypeFilter<IInputActionRx>()
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
                if (prop.GetValue(this).Is<object>(out var propValue)
                    &&
                    propValue is IDisposable propValueDisposable)
                {
                    propValueDisposable.Dispose();
                }

                action = InputActionRxFactory.Create(prop.PropertyType, ResolveInputAction(prop));
                prop.SetValue(this, action);
                RegsiterAction(action);
            }

            registeredActions.TrimExcess();
            CCDebug.Instance.PrintLog("Initialized", new DebugContext(GetType()).Additive());
        }
    }
}

