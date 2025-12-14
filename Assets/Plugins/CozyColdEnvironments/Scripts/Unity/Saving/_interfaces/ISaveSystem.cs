using CCEnvs.Snapshots;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.Unity.Saving
{
    public interface ISaveSystem
    {
        UniTask SaveAsync(string path);

        UniTask LoadAsync(string path);

        /// <returns>Disposable after dispose that invokes <see cref="UnbindObject(object?, Scene)"/></returns>
        IDisposable BindObject(object obj, SceneInfo sceneInfo);
        /// <inheritdoc cref="BindObject(object, SceneInfo)"/>
        IDisposable BindObject(object obj, Scene scene);

        bool UnbindObject(object? obj, SceneInfo scene);
        bool UnbindObject(object? obj, Scene scene);

        void RegisterType(Type type, Func<object, ISnapshot> serializableConverter);

        bool UnregisterType(Type type);

        bool IsTypeRegistered(Type? type);
    }
}
