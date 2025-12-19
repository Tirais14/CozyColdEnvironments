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

        /// <summary>
        /// Hierarchy path will be used as runtime id or from component <see cref="Components.RuntimeId"/>
        /// </summary>
        /// <returns>Disposable which invokes <see cref="UnregisterObject(object?)"/></returns>
        IDisposable RegisterGameObject(GameObject gameObject);

        /// <inheritdoc cref="RegisterObject(object, string, SceneInfo?)"/>
        IDisposable RegisterGameObject(GameObject gameObject, string key);

        bool UnregisterObject(object? obj);

        void RegisterType(Type type, Func<object, ISnapshot> converter);
        void RegisterType<T>(Func<T, ISnapshot> converter);

        bool UnregisterType(Type? type);
        bool UnregisterType<T>();

        bool IsTypeRegistered(Type? type);
        bool IsTypeRegistered<T>();
    }
}
