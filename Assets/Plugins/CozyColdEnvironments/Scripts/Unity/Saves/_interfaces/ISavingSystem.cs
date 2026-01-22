using CCEnvs.FuncLanguage;
using CCEnvs.Snapshots;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public interface ISavingSystem
    {
        string? LoadedFileDataRaw { get; }
        Maybe<SaveFileData> LoadedFileData { get; }

        bool IsSaving { get; }
        bool IsSaveLoading { get; }

        UniTask SaveInFileAsync(string path, CancellationToken cancellationToken = default);

        UniTask<string> LoadFromFileAsync(string path, CancellationToken cancellationToken = default);

        UniTask ApplySaveFileDataAsync(SaveFileData saveFileData, CancellationToken cancellationToken = default);

        UniTask<SaveFileData> CaptureSaveDataAsync(CancellationToken cancellationToken = default);

        UniTask<string> CaptureSerializedSaveDataAsync(CancellationToken cancellationToken = default);

        /// <returns>Disposable which initiates unregistering</returns>
        IDisposable RegisterObject<TObject>(
            TObject obj,
            string key,
            SceneInfo sceneInfo = default)
            where TObject : class;

        /// <returns>Disposable which initiates unregistering</returns>
        IDisposable RegisterObject<TObject>(
            TObject obj,
            Func<TObject, string> keySelector,
            SceneInfo sceneInfo = default)
            where TObject : class;

        /// <returns>Disposable which initiates unregistering</returns>
        IDisposable RegisterObject<TObject, TState>(
            TObject obj,
            TState state,
            Func<TObject, TState, string> keySelector,
            SceneInfo sceneInfo = default)
            where TObject : class;

        /// <summary>
        /// Use as key <see cref="Components.RuntimeId.Id"/> or create it and set id by hierarchy path
        /// </summary>
        /// <returns>Disposable which initiates unregistering</returns>
        IDisposable RegisterUnityObject(Component component, SceneInfo sceneInfo = default);

        /// <inheritdoc cref="RegisterUnityObject(Component)"/>
        IDisposable RegisterUnityObject(GameObject gameObject, SceneInfo sceneInfo = default);

        bool UnregisterObject(object? obj, SceneInfo sceneInfo = default);

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

        bool IsInstanceRegistered(object? obj, SceneInfo sceneInfo = default);

        bool TryRestoreInstanceFromMemory(object obj, string key, SceneInfo byScene = default);
    }
}
