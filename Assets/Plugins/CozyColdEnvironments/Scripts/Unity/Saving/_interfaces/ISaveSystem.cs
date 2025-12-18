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

        /// <returns>Disposable after dispose that invokes <see cref="UnbindObject(object?)"/></returns>
        IDisposable BindObject(object obj, SceneInfo? sceneInfo = null);

        /// <inheritdoc cref="BindObject(object, SceneInfo?)"/>
        IDisposable BindGameObject(GameObject gameObject, string runtimeId);

        /// <inheritdoc cref="BindObject(object, SceneInfo?)"/>
        IDisposable BindGameObject(GameObject gameObject, Func<GameObject, string> runtimeIdSelector);

        bool UnbindObject(object? obj);

        void RegisterType(Type type, Func<object, ISnapshot> serializableConverter);
        void RegisterType<T>(Func<object, ISnapshot> serializableConverter);

        bool UnregisterType(Type type);
        bool UnregisterType<T>();

        bool IsTypeRegistered(Type? type);
        bool IsTypeRegistered<T>();
    }
}
