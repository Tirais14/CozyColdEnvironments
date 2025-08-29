using System;
using UnityEngine;
using CCEnvs.Diagnostics;

#nullable enable
namespace CCEnvs.Disposables
{
    public static class DisposableExtensions
    {
        /// <returns>self</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IDisposable BindTo(this IDisposable value, Component component)
        {
            if (value.IsNull())
                throw new ArgumentNullException(nameof(value));
            if (component == null)
                throw new ArgumentNullException(nameof(component));

            if (!component.TryGetComponent<GameObjectDisposablesObserver>(out var observer))
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

            if (!gameObject.TryGetComponent<GameObjectDisposablesObserver>(out var observer))
                observer = gameObject.AddComponent<GameObjectDisposablesObserver>();

            observer.BindDisposable(value);
            return value;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static T AddTo<T>(this T value,
                                 IDisposableCollection collection)
            where T : IDisposable
        {
            if (collection.IsNull())
                throw new ArgumentNullException(nameof(collection));

            collection.Add(value);

            return value;
        }
        /// <exception cref="ArgumentNullException"></exception>
        public static T AddTo<T>(this T value,
                                 IDisposableContainer container)
            where T : IDisposable
        {
            if (container.IsNull())
                throw new ArgumentNullException(nameof(container));

            container.Add(value);

            return value;
        }
        public static IDisposable AddTo(this IDisposable value,
                                        IDisposableContainer container)
        {
            return value.AddTo<IDisposable>(container);
        }
    }
}
