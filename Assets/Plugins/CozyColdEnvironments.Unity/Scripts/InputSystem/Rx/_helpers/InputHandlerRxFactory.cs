using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable
namespace CCEnvs.Unity.InputSystem.Rx
{
    public static class InputHandlerRxFactory
    {
        public static IList<IInputHandlerRx> CreateInputHandlers(
            string resourcesInputAcitonAssetPath,
            (string ActionMapName, Type InputHandlerType)[] inputHandlerInfos
            )
        {
            Guard.IsNotNull(resourcesInputAcitonAssetPath, nameof(resourcesInputAcitonAssetPath));
            Guard.IsNotNull(inputHandlerInfos, nameof(inputHandlerInfos));

            var inputActionAsset = Resources.Load<InputActionAsset>(resourcesInputAcitonAssetPath);

            var handlerCtors =
                from info in inputHandlerInfos
                let actionMap = inputActionAsset.FindActionMap(info.ActionMapName, throwIfNotFound: true)
                select (value: info, actionMap) into info
                let inputHandlerType = info.value.InputHandlerType
                let ctor = inputHandlerType.GetConstructor(BindingFlagsDefault.InstanceAll, null, new Type[] { typeof(InputActionMap) }, Array.Empty<ParameterModifier>())
                    ??
                    inputHandlerType.GetConstructor(BindingFlagsDefault.InstanceAll, null, new Type[] { typeof(InputActionMap), typeof(bool) }, Array.Empty<ParameterModifier>())
                select (ctor, info.actionMap);

            var handlers = new List<IInputHandlerRx>();

            IInputHandlerRx handler;

            int i = 0;

            foreach (var (ctor, actionMap) in handlerCtors)
            {
                if (ctor is null)
                    throw new InvalidOperationException($"Cannot find any constructor. Type: {inputHandlerInfos[i].InputHandlerType}");

                if (ctor.GetParameters().Length > 1)
                    handler = (IInputHandlerRx)ctor.Invoke(new object[] { actionMap, true });
                else
                    handler = (IInputHandlerRx)ctor.Invoke(new object[] { actionMap });

                handlers.Add(handler);

                i++;
            }

            return handlers;
        }
    }
}
