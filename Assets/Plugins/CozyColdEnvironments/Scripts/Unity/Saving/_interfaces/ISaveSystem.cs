using CCEnvs.FuncLanguage;
using CCEnvs.Snapshots;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Saving
{
    public interface ISaveSystem
    {
        UniTask SaveAsync(string path);

        UniTask LoadAsync(string path);

        /// <summary>Only for reference types</summary>
        /// <returns>Disposable which invokes <see cref="UnregisterObject(object?)"/></returns>
        IDisposable RegisterObject(object obj, string key, SceneInfo? sceneInfo = null);

        /// <inheritdoc cref="RegisterObject(object, string, SceneInfo?)"/>
        IDisposable RegisterObject<TObject, TState>(
            TObject obj,
            Func<TObject, Maybe<TState>, string> keySelector,
            SceneInfo? sceneInfo = default,
            TState? state = default)
            where TObject : class;

        /// <returns>Disposable which invokes <see cref="UnregisterObject(object?)"/></returns>
        IDisposable RegisterGameObject(GameObject gameObject, string runtimeId);

        /// <inheritdoc cref="RegisterGameObject(GameObject, string)"/>
        IDisposable RegisterGameObject<TState>(
            GameObject gameObject,
            Func<GameObject, Maybe<TState>, string> runtimeIdSelector,
            TState? state = default);

        /// <summary>
        /// Hierarchy path will be used as runtime id or from component <see cref="Components.RuntimeId"/>
        /// </summary>
        /// <returns>Disposable which invokes <see cref="UnregisterObject(object?)"/></returns>
        IDisposable RegisterGameObject(GameObject gameObject);

        bool UnregisterObject(object? obj);

        void RegisterType(Type type, Func<object, ISnapshot> converter);
        void RegisterType<T>(Func<T, ISnapshot> converter);

        bool UnregisterType(Type? type);
        bool UnregisterType<T>();

        bool IsTypeRegistered(Type? type);
        bool IsTypeRegistered<T>();
    }
}
