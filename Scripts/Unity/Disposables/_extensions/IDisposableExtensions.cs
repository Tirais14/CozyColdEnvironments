#nullable enable
using CCEnvs.Diagnostics;
using System;
using UnityEngine;

namespace CCEnvs.Unity.Disposables
{
    public static class IDisposableExtensions
    {
        /// <returns>self</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IDisposable BindTo(this IDisposable value, Component component)
        {
            if (value.IsNull())
                throw new ArgumentNullException(nameof(value));
            if (component == null)
                throw new ArgumentNullException(nameof(component));

            if (!component.TryGetComponent<GameObjectDisposablesObserver>(out GameObjectDisposablesObserver? observer))
                observer = component.gameObject.AddComponent<GameObjectDisposablesObserver>();

            observer.BindDisposable(value);
            return value;
        }
        /// <returns>self</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IDisposable BindTo(this IDisposable value, GameObject gameObject)
        {
            if (value.IsNull())
                throw new ArgumentNullException(nameof(value));
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            if (!gameObject.TryGetComponent<GameObjectDisposablesObserver>(out GameObjectDisposablesObserver? observer))
                observer = gameObject.AddComponent<GameObjectDisposablesObserver>();

            observer.BindDisposable(value);
            return value;
        }
    }
}
