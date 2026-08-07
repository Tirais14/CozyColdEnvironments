using CCEnvs.Disposables;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public static class DropTargetRegistry
    {
        private static Dictionary<IEventHandler, DropTarget> targets = new(0);

        public static IReadOnlyDictionary<IEventHandler, DropTarget> Targets => targets;

        public static bool Unregister(IEventHandler handler)
        {
            return targets.Remove(handler);
        }

        public static DisposableLight<IEventHandler> Register(
            IEventHandler handler,
            GameObject gameObject
            )
        {
            CC.Guard.IsNotNull(handler, nameof(handler));
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));

            targets.Add(handler, new DropTarget(gameObject));

            return CCDisposable.CreateLight(handler, static handler => Unregister(handler));
        }
    }
}
