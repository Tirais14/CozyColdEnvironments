using CCEnvs.Snapshots;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Saving
{
    public interface ISavingSystem
    {
        UniTask SaveAsync(string path);

        UniTask LoadAsync(string path);

        /// <returns>Disposable which initiates unregistering</returns>
        IDisposable RegisterObject(object obj, string key, SceneInfo? sceneInfo = null);

        /// <summary>
        /// Use as key <see cref="Components.RuntimeId.Id"/> or create it and set id by hierarchy path
        /// </summary>
        /// <returns>Disposable which initiates unregistering</returns>
        IDisposable RegisterGameObject(GameObject gameObject);

        /// <inheritdoc cref="RegisterObject(object, string, SceneInfo?)"/>
        IDisposable RegisterGameObject(GameObject gameObject, string key);

        /// <inheritdoc cref="RegisterGameObject(GameObject)"/>
        IDisposable RegisterComponent(Component component);

        /// <inheritdoc cref="RegisterGameObject(GameObject, string)"/>
        IDisposable RegisterComponent(Component component, string key);

        bool UnregisterObject(object? obj);

        /// <param name="type">Must be reference type</param>
        void RegisterType(Type type, Func<object, ISnapshot> converter);

        void RegisterType<T>(Func<T, ISnapshot> converter) where T : class;

        /// <summary>
        /// On successfull unregistered the type all objects will be unregistered also
        /// </summary>
        bool UnregisterType(Type? type);

        /// <inheritdoc cref="UnregisterType(Type?)"/>
        bool UnregisterType<T>() where T : class;

        bool IsTypeRegistered(Type? type);
        bool IsTypeRegistered<T>();

        bool IsInstanceRegistered(object? obj, SceneInfo? sceneInfo = null);
    }
}
