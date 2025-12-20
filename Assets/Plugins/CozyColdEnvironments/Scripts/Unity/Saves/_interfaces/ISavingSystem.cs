using CCEnvs.Snapshots;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public interface ISavingSystem
    {
        UniTask SaveAsync(string path);

        UniTask LoadAsync(string path);

        /// <returns>Disposable which initiates unregistering</returns>
        IDisposable RegisterObject<TObject>(
            TObject obj,
            string key,
            SceneInfo? sceneInfo = null)
            where TObject : class;

        /// <returns>Disposable which initiates unregistering</returns>
        IDisposable RegisterObject<TObject>(
            TObject obj,
            Func<TObject, string> keySelector,
            SceneInfo? sceneInfo = null)
            where TObject : class;

        /// <returns>Disposable which initiates unregistering</returns>
        IDisposable RegisterObject<TObject, TState>(
            TObject obj,
            TState state,
            Func<TObject, TState, string> keySelector,
            SceneInfo? sceneInfo = null)
            where TObject : class;

        ///// <summary>
        ///// Use as key <see cref="Components.RuntimeId.Id"/> or create it and set id by hierarchy path
        ///// </summary>
        ///// <returns>Disposable which initiates unregistering</returns>
        //IDisposable RegisterGameObject(GameObject gameObject);

        ///// <inheritdoc cref="RegisterObject(object, string, SceneInfo?)"/>
        //IDisposable RegisterGameObject(GameObject gameObject, string key);

        ///// <inheritdoc cref="RegisterObject(object, string, SceneInfo?)"/>
        //IDisposable RegisterGameObject(GameObject gameObject, Func<GameObject, string> keySelector);

        ///// <inheritdoc cref="RegisterObject(object, string, SceneInfo?)"/>
        //IDisposable RegisterGameObject<TState>(GameObject gameObject, TState state, Func<GameObject, TState, string> keySelector);

        ///// <inheritdoc cref="RegisterGameObject(GameObject)"/>
        //IDisposable RegisterComponent(Component component);

        ///// <inheritdoc cref="RegisterGameObject(GameObject, string)"/>
        //IDisposable RegisterComponent(Component component, string key);

        ///// <inheritdoc cref="RegisterGameObject(GameObject, string)"/>
        //IDisposable RegisterComponent(Component component, Func<Component, string> keySelector);

        ///// <inheritdoc cref="RegisterGameObject(GameObject, string)"/>
        //IDisposable RegisterComponent<TState>(Component component, TState state, Func<Component, TState, string> keySelector);

        /// <summary>
        /// Use as key <see cref="Components.RuntimeId.Id"/> or create it and set id by hierarchy path
        /// </summary>
        /// <returns>Disposable which initiates unregistering</returns>
        IDisposable RegisterUnityObject(Component component);

        /// <inheritdoc cref="RegisterUnityObject(Component)"/>
        IDisposable RegisterUnityObject(GameObject gameObject);

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
