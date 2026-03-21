using CCEnvs.Attributes;
using CCEnvs.Collections;
using CCEnvs.Dependencies;
using CCEnvs.Reflection;
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable
namespace CCEnvs.Unity.InputSystem.Rx
{
    internal static class InputActionRxReference
    {
        internal static readonly Dictionary<(string ActionName, Type HandlerType), InputActionRx> resolvedInputActions = new(0);
        internal static readonly Dictionary<Type, PropertyInfo[]> handlerActionProperties = new(0);
        internal static readonly Dictionary<InputHandlerRx, ImmutableDictionary<string, InputActionRx>> resolvedActions = new();
        internal static readonly Dictionary<string, Type> resolveHandlerTypes = new(0);

        internal static Type[] handlerTypes = Type.EmptyTypes;

        [OnInstallExecutable]
        private static void OnInstall(MemberInfo[] domainMembers)
        {
            handlerTypes =
                (from member in domainMembers.AsParallel()
                 where member.MemberType.IsFlagSetted(MemberTypes.TypeInfo)
                 ||
                 member.MemberType.IsFlagSetted(MemberTypes.NestedType)
                 select (Type)member into type
                 where type.IsType<InputHandlerRx>()
                 select type
                 )
                 .ToArray();
        }
    }

    [Serializable]
    public class InputActionRxReference<TInputValue>
        where TInputValue : struct
    {
        [SerializeField]
        protected string inputHandlerName = null!;

        [SerializeField]
        protected string actionName = null!;

        private InputHandlerRx? inputHandler;

        private InputActionRx? value;

        public string ActionName => actionName;

        public InputHandlerRx InputHandler => ResolveInputHandler();

        public InputActionRx<TInputValue> Value => ResolveInputAction();

        public bool IsValid => actionName.IsNotNullOrWhiteSpace() && inputHandlerName.IsNotNullOrWhiteSpace();

        private InputHandlerRx ResolveInputHandler()
        {
            if (!InputActionRxReference.resolveHandlerTypes.TryGetValue(inputHandlerName, out var handlerType))
            {
                handlerType = InputActionRxReference.handlerTypes.Find(type => type.Name.StartsWith(inputHandlerName))
                    ??
                    throw new InvalidOperationException($"Cannot find input handler. Input Handler Name: {inputHandlerName}");
            }

            inputHandler ??= (InputHandlerRx)CCServices.Resolve(handlerType);

            return inputHandler;
        }

        private InputActionRx<TInputValue> ResolveInputAction()
        {
            var handlerType = InputHandler.GetType();

            if (InputActionRxReference.resolvedInputActions.TryGetValue(
                (actionName, handlerType),
                out var inputAction)
                )
            {
                return (InputActionRx<TInputValue>)inputAction;
            }

            if (!InputActionRxReference.handlerActionProperties.TryGetValue(
                handlerType, 
                out var handlerActionProps)
                )
            {
                handlerActionProps = handlerType.GetProperties(BindingFlagsDefault.InstancePublic)
                    .Where(prop => prop.CanRead && prop.PropertyType.IsType<InputActionRx>())
                    .ToArray();

                InputActionRxReference.handlerActionProperties[handlerType] = handlerActionProps;
            }

            if (!InputActionRxReference.resolvedActions.TryGetValue(
                InputHandler,
                out var actions
                ))
            {
                actions = (from prop in handlerActionProps
                           let actionName = prop.Name
                           let action = (InputActionRx)prop.GetValue(inputHandler)
                           select (actionName, action)
                           )
                           .ToDictionary()
                           .ToImmutableDictionary();

                InputActionRxReference.resolvedActions[InputHandler] = actions;
            }

            return (InputActionRx<TInputValue>)actions[actionName];
        }
    }

    [Serializable]
    public class ButtonActionRxReference : InputActionRxReference<bool>
    {
        public new ButtonActionRx Value => (ButtonActionRx)base.Value;
    }
}
