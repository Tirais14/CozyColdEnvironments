using CCEnvs.Collections;
using CCEnvs.Services;
using CCEnvs.Diagnostics;
using CCEnvs.Linq;
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
namespace CCEnvs.UnityX.InputSystem.Rx
{
    public abstract class InputHandlerRx
        :
        IInputHandlerRx
    {
        protected readonly List<IDisposable> disposables = new();

        private readonly Dictionary<string, IInputActionRx> registeredActions = new(0);

        private readonly ReactiveProperty<bool> isEnabled;

        public InputActionMap ActionMap { get; }

        public bool IsEnabled => isEnabled.Value && ActionMap.enabled;

        protected InputHandlerRx(InputActionMap actionMap, bool autoSetProps)
        {
            ActionMap = actionMap;
            isEnabled = new ReactiveProperty<bool>(actionMap.enabled);

            PropertyInfo[]? actionProps = null;

            if (autoSetProps)
                actionProps = SetProperties();

            Enable();
            Type type = GetType();
            BindToServices(type);
            BindActionsToServices(type,
                actionProps
                ??
                type.GetProperties(BindingFlagsDefault.InstancePublic)
                .Where(prop => prop.PropertyType.IsType<IInputActionRx>())
                .ToArray()
                );
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

        public Observable<bool> ObserveEnabled()
        {
            return isEnabled.Where(static x => x);
        }

        public Observable<bool> ObserveDisabled()
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

            registeredActions.Add(inputAction.Name, inputAction);
            disposables.Add(inputAction);
        }

        private void BindToServices(Type type)
        {
            CCServices.Bind(type).FromInstance(this).AsSingle();
            CCServices.Bind(type)
                .WithID(type.GetName(TypeNameConvertingAttributes.None))
                .FromInstance(this)
                .WithInterfaces(nameof(IInputHandlerRx))
                .IfNotBound()
                .AsSingle();
        }

        private void BindActionsToServices(Type type, PropertyInfo[] actionProps)
        {
            foreach (var item in actionProps.Where(prop => prop.PropertyType.IsType<IInputActionRx>())
                .Select(this, static (prop, @this) => prop.GetValue(@this))
                .OfType<IInputActionRx>())
            {
                CCServices.Bind(item.GetType())
                    .WithID(type.GetName(TypeNameConvertingAttributes.None) + '.' + item.Name)
                    .FromInstance(item)
                    .WithInterfaces(nameof(IInputActionRx))
                    .IfNotBound()
                    .AsSingle();
            }
        }

        private InputAction ResolveInputAction(PropertyInfo prop)
        {
            InputAction resolved = ActionMap.FindAction(prop.Name, throwIfNotFound: true);

            return resolved;
        }

        private PropertyInfo[] SetProperties()
        {
            PropertyInfo[] props = GetType()
                .GetProperties(BindingFlagsDefault.InstancePublic)
                .Where(prop => prop.PropertyType.IsType<IInputActionRx>())
                .ToArray();

            if (props.IsNullOrEmpty())
            {
                CCDebug.Instance.PrintWarning("Cannot find any input action properties.");
                return Array.Empty<PropertyInfo>();
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
            return props;
        }
    }
}

