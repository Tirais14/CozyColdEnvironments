using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Pools;
using CCEnvs.Snapshots;
using CCEnvs.Unity.Components;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using R3;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public sealed partial class SavingSystem : CCBehaviourStaticPublic<SavingSystem>, ISavingSystem
    {
        private readonly Type gameObjectType = typeof(GameObject);

        private readonly Dictionary<Type, HashSet<RegisteredObject>> objectLists = new();

        private readonly Dictionary<RegisteredObject, object> objectKeys = new();

        private readonly Dictionary<Type, Func<object, ISnapshot>> converters = new();
        private readonly Dictionary<SceneInfo?, CompositeDisposable> sceneDisposables = new();

        private readonly Dictionary<LoadedSnapshotKey, List<ISnapshot>> loadedSnapshotLists = new();

        private UnityAction<Scene> onSceneUnloaded;

        protected override void Awake()
        {
            base.Awake();
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            SusbcribeSceneUnloaded();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SceneManager.sceneUnloaded -= onSceneUnloaded;
            onSceneUnloaded = null!;
        }

        private static string InstanceRegisterdMessage(object obj)
        {
            return $"Instance \"{obj}\" already registered";
        }

        public IDisposable RegisterObject<TObject>(
            TObject obj,
            string key,
            SceneInfo? sceneInfo = null) 
            where TObject : class
        {
            CC.Guard.IsNotNull(obj, nameof(obj));
            Guard.IsNotNullOrWhiteSpace(key, nameof(key));

            return RegisterObjectInternal(obj, obj.GetType(), key, sceneInfo);
        }

        public IDisposable RegisterObject<TObject>(
            TObject obj,
            Func<TObject, string> keySelector,
            SceneInfo? sceneInfo = null)
             where TObject : class
        {
            CC.Guard.IsNotNull(obj, nameof(obj));
            Guard.IsNotNull(keySelector, nameof(keySelector));

            var keyFactory = new KeyFactory<TObject>(obj, keySelector);

            return RegisterObjectInternal(obj, obj.GetType(), keyFactory, sceneInfo);
        }

        public IDisposable RegisterObject<TObject, TState>(
            TObject obj,
            TState state,
            Func<TObject, TState, string> keySelector,
            SceneInfo? sceneInfo = null)
            where TObject : class
        {
            CC.Guard.IsNotNull(obj, nameof(obj));
            Guard.IsNotNull(keySelector, nameof(keySelector));

            var keyFactory = new KeyFactory<TObject, TState>(obj, state, keySelector);

            return RegisterObjectInternal(obj, obj.GetType(), keyFactory, sceneInfo);
        }

        public IDisposable RegisterUnityObject(Component component)
        {
            CC.Guard.IsNotNull(component, nameof(component));

            object keyOrFactory = ResolveKeyForUnityObject(component);
            SceneInfo sceneInfo = component.gameObject.scene.GetSceneInfo();

            return RegisterObjectInternal(component, component.GetType(), keyOrFactory, sceneInfo);
        }

        public IDisposable RegisterUnityObject(GameObject gameObject)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));

            object keyOrFactory = ResolveKeyForUnityObject(gameObject);
            SceneInfo sceneInfo = gameObject.scene.GetSceneInfo();

            return RegisterObjectInternal(gameObject, gameObjectType, keyOrFactory, sceneInfo);
        }

        public bool UnregisterObject(object? obj)
        {
            if (obj is null)
                return false;

            SceneInfo? objSceneInfo = ResolveSceneInfo(obj);
            RegisteredObject regObj = new RegisteredObject(obj, objSceneInfo, converters);

            return UnregisterObjectInternal(regObj);
        }

        private bool UnregisterObjectInternal(RegisteredObject regObj)
        {
            objectKeys.Remove(regObj);

            if (!objectLists.TryGetValue(regObj.ObjectType, out var objs))
                return false;

            return objs.Remove(regObj);
        }

        public async UniTask LoadAsync(string path)
        {
            SaveFileData saveFile = await LoadSaveFileAsync(path);
            IList<SaveFileSceneData> notRestoredSceneDatas = saveFile.RestoreLoadedScenes();
            RegisterLoadedSnapshots(notRestoredSceneDatas);
        }

        public async UniTask SaveAsync(string path)
        {
            SaveFileData saveFileData = await BuildSaveFileDataAsync();
            string serialized = JsonConvert.SerializeObject(saveFileData, CC.JsonSettings);

            await UniTask.SwitchToThreadPool();
            await File.WriteAllTextAsync(path, serialized, cancellationToken: destroyCancellationToken);
            await UniTask.SwitchToMainThread();
        }

        public void RegisterType(Type type, Func<object, ISnapshot> converter)
        {
            Guard.IsNotNull(type, nameof(type));
            Guard.IsTrue(type.IsClass, nameof(type), "Is not reference type");

            if (IsTypeRegistered(type))
                throw new InvalidOperationException($"Type: {type} already registered.");

            Guard.IsNotNull(converter, nameof(converter));

            converters.Add(type, converter);
        }
        public void RegisterType<T>(Func<T, ISnapshot> converter) where T : class
        {
            RegisterType(typeof(T), (obj) => converter((T)obj));
        }

        public bool UnregisterType(Type? type)
        {
            if (type is null)
                return false;

            converters.Remove(type);
            return objectLists.Remove(type);
        }
        public bool UnregisterType<T>() where T : class
        {
            return UnregisterType(typeof(T));
        }

        public bool IsTypeRegistered(Type? type)
        {
            if (type is null)
                return false;

            return converters.ContainsKey(type);
        }
        public bool IsTypeRegistered<T>()
        {
            return IsTypeRegistered(typeof(T));
        }

        public bool IsInstanceRegistered(object? obj, SceneInfo? sceneInfo = null)
        {
            if (obj is null)
                return false;

            if (!IsTypeRegistered(obj.GetType()))
                return false;

            var regObj = new RegisteredObject(obj, sceneInfo ?? ResolveSceneInfo(obj), converters);
            return IsInstanceRegisteredInternal(regObj);
        }

        private bool IsInstanceRegisteredInternal(RegisteredObject regObj)
        {
            if (!objectKeys.ContainsKey(regObj))
            {
                if (!objectLists.TryGetValue(regObj.ObjectType, out var objList))
                    return false;

                return objList.Contains(regObj);
            }

            return true;
        }

        private SceneInfo? ResolveSceneInfo(object obj)
        {
            return obj switch
            {
                GameObject go => go.scene.GetSceneInfo(),
                Component cmp => cmp.gameObject.scene.GetSceneInfo(),
                UnityEngine.Object uObj => GameObject.GetScene(uObj.GetInstanceID()).GetSceneInfo(),
                _ => null,
            };
        }

        private async UniTask<PooledArray<RegisteredObject>> GetSceneObjectsAsync(SceneInfo sceneInfo)
        {
            await UniTask.SwitchToThreadPool();

            using var _ = ListPool<RegisteredObject>.Get(out var results);
            foreach (var regObj in objectLists.Values.SelectMany(x => x))
            {
                if (regObj.SceneInfo != sceneInfo)
                    continue;

                results.Add(regObj);
            }

            await UniTask.SwitchToMainThread();
            return results.ToArrayPooled();
        }

        private async UniTask<PooledArray<SaveFileSceneData>> BuildSceneDatasAsync()
        {
            using var __ = ListPool<KeyedSnapshot<ISnapshot>>.Get(out var snapshots);
            using var _ = ListPool<SaveFileSceneData>.Get(out var sceneDatas);

            ISnapshot snapshot;
            KeyedSnapshot<ISnapshot> keyedSnapshot;
            SaveFileSceneData sceneData;
            Maybe<string> objKey;
            foreach (var sceneInfo in SceneManagerHelper.GetLoadedScenes().Select(x => x.GetSceneInfo()))
            {
                using PooledArray<RegisteredObject> regObjects = await GetSceneObjectsAsync(sceneInfo);
                foreach (var regObj in regObjects.Value)
                {
                    try
                    {
                        snapshot = regObj.CreateSnapshot();
                        objKey = GetKey(regObj);

                        if (!objKey.TryGetValue(out string? key))
                        {
                            this.PrintError($"Missing key of registered object\"{regObj}\"");
                            continue;
                        }

                        keyedSnapshot = new KeyedSnapshot<ISnapshot>(snapshot, key);
                        snapshots.Add(keyedSnapshot);
                    }
                    catch (Exception ex)
                    {
                        this.PrintError($"Registered object \"{regObj}\" throwed exception and will be skipped. {ex.GetType()}: {ex}");
                    }
                }

                sceneData = new SaveFileSceneData(sceneInfo, snapshots.Cast<KeyedSnapshot<ISnapshot>>().ToArray());
                sceneDatas.Add(sceneData);
                snapshots.Clear();
            }

            return sceneDatas.ToArrayPooled();
        }

        private async UniTask<SaveFileData> BuildSaveFileDataAsync()
        {
            using PooledArray<SaveFileSceneData> sceneDatas = await BuildSceneDatasAsync();
            return new SaveFileData("0.0.0.0", sceneDatas.Value.ToImmutableArray()); //TODO: Versioning
        }

        private object ResolveKeyForUnityObject(UnityEngine.Object uObject)
        {
            switch (uObject)
            {
                case GameObject go:
                    {
                        return new KeyFactory<GameObject>(go,
                            static (go) =>
                            {
                                if (go.TryGetComponent<PersistentGuid>(out var guidCmp)
                                    &&
                                    guidCmp.Guid.IsNotNullOrWhiteSpace())
                                {
                                    return guidCmp.Guid;
                                }

                                if (go.TryGetComponent<RuntimeId>(out var idCmp))
                                {
                                    if (idCmp.Id.IsNullOrWhiteSpace())
                                        return go.GetHierarchyPath();

                                    return idCmp.Id;
                                }

                                return go.GetHierarchyPath();
                            });
                    }
                case Component cmp:
                    return ResolveKeyForUnityObject(cmp.gameObject);
                default:
                    throw CC.ThrowHelper.InvalidOperationException(uObject.GetType());
            }
        }

        private IDisposable RegisterObjectInternal(
            object obj,
            Type objType,
            object keyOrFactory,
            SceneInfo? sceneInfo = null)
        {
            CC.Guard.IsNotNull(obj, nameof(obj));
            Guard.IsNotNull(objType, nameof(objType));
            CC.Guard.IsNotNull(keyOrFactory, nameof(keyOrFactory));

            if (!IsTypeRegistered(objType))
            {
                this.PrintError($"Type: {objType} is not registered");
                return Disposable.Empty;
            }

            sceneInfo ??= ResolveSceneInfo(obj);
            var regObj = new RegisteredObject(obj, sceneInfo, converters);

            if (IsInstanceRegisteredInternal(regObj))
            {
                this.PrintError(InstanceRegisterdMessage(obj));
                return Disposable.Empty;
            }

            objectKeys.Add(regObj, keyOrFactory);

            var sysObjects = objectLists.GetOrCreateNew(objType);

            sysObjects.Add(regObj);

            var disposables = sceneDisposables.GetOrCreateNew(sceneInfo);

            return new Registration(this, regObj).AddTo(disposables);
        }

        private Maybe<string> GetKey(RegisteredObject regObj)
        {
            if (!objectKeys.TryGetValue(regObj, out object keyUntyped))
                return Maybe<string>.None;

            switch (objectKeys[regObj])
            {
                case string key:
                    return key;
                case IKeyFactory keyFactory:
                    return keyFactory.CreateKey();
                default:
                    throw CC.ThrowHelper.InvalidOperationException(keyUntyped, nameof(keyUntyped));
            }
        }

        private void SusbcribeSceneUnloaded()
        {
            onSceneUnloaded = scene =>
            {
                SceneInfo sceneInfo = scene.GetSceneInfo();

                if (!sceneDisposables.Remove(sceneInfo, out var disposables))
                    return;

                disposables.Dispose();
            };

            SceneManager.sceneUnloaded += onSceneUnloaded;
        }

        private async UniTask<SaveFileData> LoadSaveFileAsync(string path)
        {
            await UniTask.SwitchToThreadPool();
            string serialized = await File.ReadAllTextAsync(path, cancellationToken: destroyCancellationToken);
            await UniTask.SwitchToMainThread();

            return JsonConvert.DeserializeObject<SaveFileData>(serialized);
        }

        private void RegisterLoadedSnapshots(IList<SaveFileSceneData> notRestoredDatas)
        {
            IList<ISnapshot> snapshotList;
            LoadedSnapshotKey loadedSnapshotKey;
            foreach (var sceneData in notRestoredDatas)
            {
                foreach (var snapshot in sceneData.Snapshots)
                {
                    if (snapshot.Key is not string snapshotKey)
                    {
                        this.PrintError($"Key \"{snapshot.Key}\" is not string. Object not restored");
                        continue;
                    }

                    loadedSnapshotKey = new LoadedSnapshotKey(snapshotKey, snapshot.TargetType, sceneData.SceneInfo);
                    snapshotList = loadedSnapshotLists.GetOrCreateNew(loadedSnapshotKey);
                    snapshotList.Add(snapshot);
                }
                
            }
        }
    }

    public static class SaveSystemExtensions
    {
        /// <inheritdoc cref="ISavingSystem.RegisterObject{TObject}(TObject, string, SceneInfo?)"/>
        public static IDisposable SavingSystemRegisterObject<TObject>(
            this TObject source,
            string key,
            SceneInfo? sceneInfo = null)
            where TObject : class
        {
            return SavingSystem.Self.RegisterObject(source, key, sceneInfo: sceneInfo);
        }

        /// <inheritdoc cref="ISavingSystem.RegisterObject{TObject}(TObject, Func{TObject, string}, SceneInfo?)"/>
        public static IDisposable SavingSystemRegisterObject<TObject>(
            this TObject source,
            Func<TObject, string> keySelector,
            SceneInfo? sceneInfo = null)
            where TObject : class
        {
            return SavingSystem.Self.RegisterObject(
                source,
                keySelector,
                sceneInfo: sceneInfo);
        }

        /// <inheritdoc cref="ISavingSystem.RegisterObject{TObject, TState}(TObject, TState, Func{TObject, TState, string}, SceneInfo?)"/>
        public static IDisposable SavingSystemRegisterObject<TObject, TState>(
            this TObject source,
            TState state,
            Func<TObject, TState, string> keySelector,
            SceneInfo? sceneInfo = null)
            where TObject : class
        {
            return SavingSystem.Self.RegisterObject(
                source,
                state,
                keySelector,
                sceneInfo: sceneInfo);
        }

        /// <inheritdoc cref="ISavingSystem.RegisterUnityObject(GameObject)"/>
        public static IDisposable SavingSystemRegisterUnityObject(this GameObject source)
        {
            return SavingSystem.Self.RegisterUnityObject(source);
        }

        /// <inheritdoc cref="ISavingSystem.RegisterUnityObject(Component)"/>
        public static IDisposable SavingSystemRegisterUnityObject(this Component source)
        {
            return SavingSystem.Self.RegisterUnityObject(source);
        }

        /// <inheritdoc cref="ISavingSystem.IsTypeRegistered(Type?)"/>
        public static bool SavingSystemIsTypeRegistered(this Type source)
        {
            return SavingSystem.Self.IsTypeRegistered(source);
        }

        /// <inheritdoc cref="ISavingSystem.IsTypeRegistered{T}()"/>
        public static bool SavingSystemIsTypeRegistered(this object source)
        {
            CC.Guard.IsNotNullSource(source);
            return source.GetType().SavingSystemIsTypeRegistered();
        }

        /// <inheritdoc cref="ISavingSystem.IsInstanceRegistered(object?, SceneInfo?)"/>
        public static bool SavingSystemIsInstanceRegistered(this object? source, SceneInfo? sceneInfo = null)
        {
            return SavingSystem.Self.IsInstanceRegistered(source, sceneInfo);
        }
    }
}
