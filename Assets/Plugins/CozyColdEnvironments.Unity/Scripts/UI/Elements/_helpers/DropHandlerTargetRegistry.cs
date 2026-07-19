using CCEnvs.Disposables;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public static class DropHandlerTargetRegistry
    {
        private static Dictionary<IEventHandler, GameObject> gameObjects = new(0);

        public static IReadOnlyDictionary<IEventHandler, GameObject> GameObjects => gameObjects;

        public static bool Unregister(IEventHandler handler)
        {
            return gameObjects.Remove(handler);
        }

        public static LightDisposable<IEventHandler> Register(
            IEventHandler handler,
            GameObject gameObject
            )
        {
            CC.Guard.IsNotNull(handler, nameof(handler));
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));

            gameObjects.Add(handler, gameObject);

            return CCDisposable.CreateLight(handler, static handler => Unregister(handler));
        }
    }
}
