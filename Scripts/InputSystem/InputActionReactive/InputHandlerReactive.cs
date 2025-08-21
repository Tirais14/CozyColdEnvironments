using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.InputSystem;
using UTIRLib.Disposables;
using UTIRLib.Reflection;

#nullable enable
namespace UTIRLib.InputSystem.Reactive
{
    public abstract class InputHandlerReactive : IInputHandlerReactive
    {
        private readonly InputActionMap actionMap;
        private bool disposed;

        protected readonly DisposableCollection disposables = new();

        protected InputHandlerReactive(InputActionMap actionMap, bool autoSetProps)
        {
            if (autoSetProps)
                SetProperties();

            this.actionMap = actionMap;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(disposing: true);
        }

        private Type? ResolveValueType(PropertyInfo prop)
        {
            if (!prop.PropertyType.IsGenericType)
                return null;

            return prop.PropertyType.GetGenericArguments()[0];
        }

        private InputAction ResolveInputAction(PropertyInfo prop)
        {
            InputAction resolved = actionMap.FindAction(nameof(prop.Name))
                ?? 
                throw new ArgumentException($"Cannot find input action with name {prop.Name}.");

            return resolved;
        }

        private void SetProperties()
        {
            IEnumerable<PropertyInfo> props =
                GetType().ForceGetProperties(BindingFlagsDefault.InstancePublic)
                         .Where(x => x.PropertyType.IsType<IInputActionReactive>());

            if (props.IsNullOrEmpty())
            {
                TirLibDebug.PrintWarning("Cannot find any input action properties.");
                return;
            }

            IInputActionReactive action;
            foreach (var prop in props)
            {
                action = InputActionReactiveFactory.Create(ResolveValueType(prop),
                                                           ResolveInputAction(prop));
                prop.SetValue(this, action);
                disposables.Add(action);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
                disposables.Dispose();

            disposed = true;
        }
    }
}
