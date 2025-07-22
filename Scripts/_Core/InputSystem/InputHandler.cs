#nullable enable
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UTIRLib.Attributes.Metadata;
using UTIRLib.Disposables;
using UTIRLib.Init;
using UTIRLib.Reflection;

namespace UTIRLib.InputSystem
{
    public class InputHandler : MonoXInitable, IInputHandler, IInitable, IDisposableContainer
    {
        private readonly DisposableCollection disposables = new();
        protected InputActionMap actionMap = null!;

        [SerializeField]
        protected InputActionAsset inputs;

        [SerializeField]
        [Tooltip("It's name must be setted in field after input action name.")]
        protected string actionPropertyNamePostfix = "Input";

        [SerializeField]
        protected string actionMapName;

        [SerializeField]
        protected InputHandlerItem[] inputActionsToInit;

        protected override void OnInit()
        {
            actionMap = inputs.FindActionMap(actionMapName, throwIfNotFound: true);

            InitInputActionProperties();
        }

        public void Dispose()
        {
            disposables.Dispose();
            GC.SuppressFinalize(this);
        }

        private PropertyInfo GetInputActionProperty(string actionName)
        {
            string propName = actionName + actionPropertyNamePostfix;

            return GetType()
                   .GetProperty(propName,
                                BindingFlagsDefault.InstanceAll)
                   ??
                   throw new Exception($"Cannot find property {propName}.");
        }

        private void SetInputActionProperty(PropertyInfo prop,
                                            Type inputValueType,
                                            string actionName)
        {
            IInputAction input = InputActionFactory.Create(inputValueType,
                                                           actionMap,
                                                           actionName);

            prop.SetValue(this, input);
        }

        private void AddToDisposables(PropertyInfo prop)
        {
            disposables.Add((IInputAction)prop.GetValue(this));
        }

        private void InitInputActionProperties()
        {
            PropertyInfo prop;

            foreach (var toInit in inputActionsToInit)
            {
                prop = GetInputActionProperty(toInit.ActionName);

                if (prop.PropertyType.IsNotType<IInputAction>())
                    throw new InvalidOperationException($"Property is not {nameof(IInputAction)} type.");

                SetInputActionProperty(prop,
                                       toInit.ValueType.GetMetaType(),
                                       toInit.ActionName);

                AddToDisposables(prop);
            }
        }
    }
}
