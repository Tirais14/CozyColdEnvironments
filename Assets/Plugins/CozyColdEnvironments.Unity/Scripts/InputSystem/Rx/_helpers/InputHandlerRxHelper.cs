using CCEnvs.Disposables;
using CCEnvs.Reflection;
using CCEnvs.Services;
using CCEnvs.UnityX.InputSystem.Rx;
using R3;
using System;
using System.Linq;

#nullable enable
namespace CCEnvs.UnityX.InputSystem
{
    public class InputHandlerRxHelper
    {
        public static IInputActionRx[] GetInputActions(IInputHandlerRx inputHandler)
        {
            CC.Guard.IsNotNull(inputHandler, nameof(inputHandler));

            return inputHandler.GetType()
                .GetProperties()
                .Where(x => x.PropertyType.IsType<IInputActionRx>())
                .Select(x => (IInputActionRx)x.GetValue(inputHandler))
                .Where(x => x.IsNotNull())
                .ToArray();
        }

        public static IDisposable RegisterInputActionsInServices(IInputHandlerRx inputHandlerRx)
        {
            var compositeDisposable = new CCEnvs.Disposables.CCCompositeDisposable();
            var inputHandlerType = inputHandlerRx.GetType();

            foreach (var inputAction in GetInputActions(inputHandlerRx))
            {
                string inputActionID = $"{inputHandlerType.GetName(TypeNameConvertingAttributes.None)}.{inputAction.Name}";

                CCServices.Bind(inputAction.GetType())
                    .FromInstance(inputAction)
                    .WithID(inputActionID)
                    .AsSingle()
                    .AddTo(compositeDisposable);

                foreach (var iface in ServiceBinderHelper.FindInterfaces<IInputActionRx>(inputAction.GetType()))
                {
                    CCServices.Bind(iface)
                        .FromInstance(inputAction)
                        .WithID(inputActionID)
                        .AsSingle()
                        .AddTo(compositeDisposable);
                }
            }

            return compositeDisposable;
        }
    }
}
