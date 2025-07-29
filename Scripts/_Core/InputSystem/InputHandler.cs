#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UTIRLib.Attributes.Metadata;
using UTIRLib.Disposables;
using UTIRLib.Reflection;

#pragma warning disable S3881
namespace UTIRLib.InputSystem
{
    public class InputHandler : MonoXInitable, IInputHandler, IDisposableContainer
    {
        private readonly DisposableCollection disposables = new();

        protected InputActionMap actionMap = null!;

        [SerializeField]
        protected InputActionAsset inputs;

        [SerializeField]
        protected string actionMapName;

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

        private void InitInputActionProperties()
        {
            PropertyInfo[] inputProps = GetType().GetProperties(BindingFlagsDefault.InstancePublic)
                                                 .Where(x => x.PropertyType.IsType<IInputAction>())
                                                 .ToArray();

            if (inputProps.Length != actionMap.actions.Count)
                throw new Exception("The number of properties does not match input actions count.");

            Dictionary<string, PropertyInfo> propsByName = inputProps.ToDictionary(x => x.Name);

            ReadOnlyArray<InputAction> inputActions = actionMap.actions;

            Type inputValueType;
            IInputAction createInputAction;
            foreach (var inputAction in inputActions)
            {
                inputValueType = inputAction.GetInputValueType();

                createInputAction = InputActionFactory.Create(inputValueType,
                                                              actionMap,
                                                              inputAction.name);

                if (!propsByName.TryGetValue(inputAction.name, out PropertyInfo prop))
                    throw new KeyNotFoundException($"Input {inputAction.name} not found.");

                prop.SetValue(this, createInputAction);
                disposables.Add(createInputAction);
            }
        }
    }
}
