using CCEnvs.Disposables;
using System.Collections.Generic;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public static class DragTargetRegistry
    {
        private static readonly Dictionary<IEventHandler, IDragTarget> targets = new();

        public static IReadOnlyDictionary<IEventHandler, IDragTarget> Targets => targets;

        public static bool Unregister(IEventHandler? eventHandler)
        {
            if (eventHandler is null)
                return false;

            return targets.Remove(eventHandler);
        }

        public static LightDisposable<IEventHandler> Register(
            IEventHandler eventHandler,
            IDragTarget target
            )
        {
            CC.Guard.IsNotNull(eventHandler, nameof(eventHandler));
            CC.Guard.IsNotNull(target, nameof(target));

            targets.Add(eventHandler, target);
            return CCDisposable.CreateLight(eventHandler, static eventHandler => Unregister(eventHandler));
        }
    }
}
