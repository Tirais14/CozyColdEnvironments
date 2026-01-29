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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public sealed partial class SavingSystem : CCBehaviourStaticPublic<SavingSystem>, ISavingSystem
    {
        private readonly Type gameObjectType = typeof(GameObject);

        private readonly Dictionary<Type, HashSet<RegisteredObject>> objectSets = new();
        private readonly Dictionary<Type, Func<object, ISnapshot>> converters = new();

        private readonly Dictionary<RegisteredObject, object> objectKeys = new();
        private readonly ConcurrentDictionary<RegisteredObjectInfo, ISnapshot> loadedSnapshots = new();
        private readonly Dictionary<SceneInfo, CompositeDisposable> sceneDisposables = new();
        private readonly ConcurrentDictionary<RegisteredObjectInfo, CancellationTokenSource> restoreInstanceTokenSources = new(); 

        private readonly ReactiveProperty<bool> isSaving = new();
        private readonly ReactiveProperty<bool> isSaveLoading = new();
        private readonly ReactiveProperty<SaveFileData> saveData = new();

        private UnityAction<Scene> onSceneUnloaded;

        public SaveFileData SaveData => saveData.Value; 

        public bool IsSaving => isSaving.Value;
        public bool IsSaveLoading => isSaveLoading.Value;

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

            isSaving.Dispose();
            isSaveLoading.Dispose();
        }

        private static string InstanceRegisterdMessage(object obj)
        {
            return $"Instance: {obj} already registered";
        }

        public IDisposable RegisterObject<TObject>(
            TObject obj,
            string key,
            SceneInfo sceneInfo = default) 
            where TObject : class
        {
            CC.Guard.IsNotNull(obj, nameof(obj));
            Guard.IsNotNullOrWhiteSpace(key, nameof(key));

            return RegisterObjectInternal(
                obj, 
                obj.GetType(),
                key, 
                sceneInfo
                );
        }

        public IDisposable RegisterObject<TObject>(
            TObject obj,
            Func<TObject, string> keySelector,
            SceneInfo sceneInfo = default)
             where TObject : class
        {
            CC.Guard.IsNotNull(obj, nameof(obj));
            Guard.IsNotNull(keySelector, nameof(keySelector));

            var keyFactory = new KeyFactory<TObject>(obj, keySelector);

            return RegisterObjectInternal(
                obj,
                obj.GetType(), 
                keyFactory, 
                sceneInfo
                );
        }

        public IDisposable RegisterObject<TObject, TState>(
            TObject obj,
            TState state,
            Func<TObject, TState, string> keySelector,
            SceneInfo sceneInfo = default)
            where TObject : class
        {
            CC.Guard.IsNotNull(obj, nameof(obj));
            Guard.IsNotNull(keySelector, nameof(keySelector));

            var keyFactory = new KeyFactory<TObject, TState>(obj, state, keySelector);

            return RegisterObjectInternal(
                obj,
                obj.GetType(),
                keyFactory,
                sceneInfo
                );
        }

        public IDisposable RegisterUnityObject(
            Component component,
            SceneInfo sceneInfo = default
            )
        {
            CC.Guard.IsNotNull(component, nameof(component));

            object keyOrFactory = GetOrCreateKeyForUnityObject(component);

            return RegisterObjectInternal(
                component,
                component.GetType(),
                keyOrFactory,
                sceneInfo
                );
        }

        public IDisposable RegisterUnityObject(
            GameObject gameObject,
            SceneInfo sceneInfo = default
            )
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));

            object keyOrFactory = GetOrCreateKeyForUnityObject(gameObject);

            return RegisterObjectInternal(
                gameObject,
                gameObjectType,
                keyOrFactory,
                sceneInfo
                );
        }

        public bool UnregisterObject(object? obj, SceneInfo sceneInfo = default)
        {
            if (obj is null)
                return false;

            var regObj = new RegisteredObject(obj, sceneInfo, converters);

            return UnregisterObjectInternal(regObj);
        }

        private bool UnregisterObjectInternal(RegisteredObject regObj)
        {
            objectKeys.Remove(regObj);

            if (!objectSets.TryGetValue(regObj.ObjectType, out var objs))
                return false;

            return objs.Remove(regObj);
        }

        public async UniTask ApplySaveFileDataAsync(
            SaveFileData saveFileData,
            CancellationToken cancellationToken = default)
        {
            var cTokenSource = cancellationToken.LinkTokens(destroyCancellationToken);

            try
            {
                await RegisterSnapshotsAsync(
                    saveFileData.SceneDatas,
                    cancellationToken: cTokenSource.Token
                    );

                ApplyRegisteredSnapshots();
            }
            finally
            {
                cTokenSource.Dispose();
            }
        }

        public async UniTask<SaveFileData> CaptureSaveDataAsync(CancellationToken cancellationToken = default)
        {
            using var cTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                self.destroyCancellationToken,
                cancellationToken
                );

            PooledArray<SaveFileSceneData> sceneDatas = await BuildSceneDatasAsync(
                    cancellationToken: cTokenSource.Token
                    );

            try
            {
                //TODO: versioning
                return new SaveFileData("0.0.0.0", sceneDatas.Value.ToImmutableArray());
            }
            finally
            {
                sceneDatas.Dispose();
                cTokenSource.Dispose();
            }
        }

        public async UniTask<string> CaptureSerializedSaveDataAsync(CancellationToken cancellationToken = default)
        {
            using var cTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                self.destroyCancellationToken,
                cancellationToken
                );

            try
            {
                var saveFileData = await CaptureSaveDataAsync(cTokenSource.Token);

                return JsonConvert.SerializeObject(
                    saveFileData,
                    CC.JsonSettings
                    );
            }
            finally
            {
                cTokenSource.Dispose();
            }
        }

        public async UniTask<string> LoadFromFileAsync(string path, CancellationToken cancellationToken = default)
        {
            if (IsSaveLoading)
                throw new InvalidOperationException("Already in save loading process");

            isSaveLoading.Value = true;
            var linkedTokenSource = cancellationToken.LinkTokens(self.destroyCancellationToken);

            try
            {
                var (saveData, saveDataRaw) = await LoadSaveFileAsync(
                    path,
                    linkedTokenSource.Token
                    );

                if (saveData.IsDefault())
                    return string.Empty;

                await ApplySaveFileDataAsync(
                    saveData,
                    linkedTokenSource.Token
                    );

                this.saveData.Value = saveData;

                return saveDataRaw;
            }
            finally
            {
                isSaveLoading.Value = false;
                linkedTokenSource.Dispose();
            }
        }

        public async UniTask LoadFromSerializedData(
            string serializedData,
            CancellationToken cancellationToken = default
            )
        {
            if (serializedData.IsNullOrWhiteSpace())
                return;

            if (IsSaveLoading)
                throw new InvalidOperationException("Already in save loading process");

            isSaveLoading.Value = true;
            var linkedTokenSource = cancellationToken.LinkTokens(self.destroyCancellationToken);

            try
            {
                var saveData = JsonConvert.DeserializeObject<SaveFileData>(serializedData, CC.JsonSettings);

                await ApplySaveFileDataAsync(saveData, linkedTokenSource.Token);

                this.saveData.Value = saveData;
            }
            finally
            {
                isSaveLoading.Value = false;
                linkedTokenSource.Dispose();
            }
        }

        public async UniTask SaveInFileAsync(string path, CancellationToken cancellationToken = default)
        {
            if (IsSaving)
                throw new InvalidOperationException("Already in saving process");

            isSaving.Value = true;
            var cTokenSource = cancellationToken.LinkTokens(destroyCancellationToken);

            try
            {
                SaveFileData saveFileData = await CaptureSaveDataAsync(cancellationToken: cTokenSource.Token);

                await RegisterSnapshotsAsync(
                    saveFileData.SceneDatas,
                    cancellationToken
                    );

                saveData.Value = saveFileData;

                string serializedsaveFileData = JsonConvert.SerializeObject(
                    saveFileData,
                    CC.JsonSettings
                    );

                await File.WriteAllTextAsync(
                    path,
                    serializedsaveFileData,
                    cancellationToken: cTokenSource.Token
                    );
            }
            finally
            {
                isSaving.Value = false;
                cTokenSource.Dispose();
            }
        }

        public async UniTask<string> SaveInMemoryAsync(CancellationToken cancellationToken = default)
        {
            if (IsSaving)
                throw new InvalidOperationException("Already in saving process");

            isSaving.Value = true;
            var cTokenSource = cancellationToken.LinkTokens(destroyCancellationToken);

            try
            {
                SaveFileData saveFileData = await CaptureSaveDataAsync(cancellationToken: cTokenSource.Token);

                await RegisterSnapshotsAsync(
                    saveFileData.SceneDatas,
                    cancellationToken
                    );

                saveData.Value = saveFileData;

                return JsonConvert.SerializeObject(
                    saveFileData,
                    CC.JsonSettings
                    );
            }
            finally
            {
                isSaving.Value = false;
                cTokenSource.Dispose();
            }
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
            return objectSets.Remove(type);
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

        public bool TryRestoreInstanceFromMemoryAsync(
            object obj,
            string key, 
            SceneInfo byScene = default
            )
        {
            return TryRestoreInstanceFromMemoryAsyncCore(
                obj,
                obj.GetType(),
                key,
                byScene
                );
        }

        public bool IsInstanceRegistered(object? obj, SceneInfo sceneInfo = default)
        {
            if (obj is null)
                return false;

            if (!IsTypeRegistered(obj.GetType()))
                return false;

            var regObj = new RegisteredObject(obj, sceneInfo, converters);
            return IsInstanceRegisteredInternal(regObj);
        }

        public Observable<bool> ObserveSavingStarted()
        {
            return isSaving.Where(static x => x);
        }

        public Observable<bool> ObserveSavingFinished()
        {
            return isSaving.Where(static x => !x);
        }

        public Observable<SaveFileData> ObserveSaveData()
        {
            return saveData;
        }

        private bool IsInstanceRegisteredInternal(RegisteredObject regObj)
        {
            if (!objectKeys.ContainsKey(regObj))
            {
                if (!objectSets.TryGetValue(regObj.ObjectType, out var objList))
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
                UnityEngine.Object uObj => GameObject.GetScene(uObj.GetEntityId()).GetSceneInfo(),
                _ => null,
            };
        }

        private async UniTask<PooledArray<RegisteredObject>> GetSceneObjectsAsync(
            SceneInfo sceneInfo,
            CancellationToken cancellationToken)
        {
            try
            {
                await UniTask.SwitchToThreadPool();

                using var _ = UnityEngine.Pool.ListPool<RegisteredObject>.Get(out var regObjs);
                int iterationsPassed = 0;

                foreach (var regObj in objectSets.Values.SelectMany(x => x))
                {
                    cancellationToken.ThrowIfCancellationRequestedByIntervalAndMoveNext(ref iterationsPassed);

                    if (regObj.SceneInfo != sceneInfo)
                        continue;

                    regObjs.Add(regObj);
                }

                return regObjs.ToArrayPooled();
            }
            finally
            {
                await UniTask.SwitchToMainThread();
            }
        }

        private async UniTask<PooledArray<SaveFileSceneData>> BuildSceneDatasAsync(CancellationToken cancellationToken)
        {
            using var __ = UnityEngine.Pool.ListPool<KeyedSnapshot<ISnapshot>>.Get(out var keyedSnapshots);
            using var _ = UnityEngine.Pool.ListPool<SaveFileSceneData>.Get(out var sceneDatas);

            KeyedSnapshot<ISnapshot> keyedSnapshot;
            SaveFileSceneData sceneData;

            var loadedScenes = SceneManagerHelper.GetLoadedSceneInfos().PrependToArray(default);

            foreach (var sceneInfo in loadedScenes)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using PooledArray<RegisteredObject> regObjects = await GetSceneObjectsAsync(
                    sceneInfo,
                    cancellationToken
                    );

                int iterationsPassed = 0;

                foreach (var regObj in regObjects.Value)
                {
                    cancellationToken.ThrowIfCancellationRequestedByIntervalAndMoveNext(ref iterationsPassed);

                    try
                    {
                        keyedSnapshot = CreateKeyedSnapshot(regObj);
                        keyedSnapshots.Add(keyedSnapshot);
                    }
                    catch (Exception ex)
                    {
                        this.PrintException(ex);
                    }
                }

                sceneData = new SaveFileSceneData(sceneInfo, keyedSnapshots.ToArray());
                sceneDatas.Add(sceneData);

                keyedSnapshots.Clear();
            }

            return sceneDatas.ToArrayPooled();

            static KeyedSnapshot<ISnapshot> CreateKeyedSnapshot(RegisteredObject regObj)
            {
                ISnapshot snapshot = regObj.CreateSnapshot();
                Maybe<string> objKey = self.ResolveKey(regObj);

                if (!objKey.TryGetValue(out string? key))
                    throw new InvalidOperationException($"Missing key of registered object\"{regObj}\"");

                return new KeyedSnapshot<ISnapshot>(snapshot, key);
            }
        }

        private object GetOrCreateKeyForUnityObject(UnityEngine.Object uObject)
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
                                        return go.GetHierarchyPath().Path;

                                    return idCmp.Id;
                                }

                                return go.GetHierarchyPath().Path;
                            });
                    }
                case Component cmp:
                    return GetOrCreateKeyForUnityObject(cmp.gameObject);
                default:
                    throw CC.ThrowHelper.InvalidOperationException(uObject.GetType());
            }
        }

        private async UniTask RestoreMonoBehaviourAfterInitialized(
            RegisteredObjectInfo regObjInfo,
            MonoBehaviour monoBeh,
            ISnapshot snapshot,
            CancellationToken cancellationToken)
        {
            try
            {
                await UniTask.WaitUntil(monoBeh,
                    monoBeh =>
                    {
                        return monoBeh.didStart;
                    },
                    timing: PlayerLoopTiming.EarlyUpdate,
                    cancellationToken: cancellationToken
                    );

                await UniTask.NextFrame(
                    timing: PlayerLoopTiming.EarlyUpdate,
                    cancellationToken: cancellationToken
                    );

                snapshot.TryRestore(monoBeh, out _);
            }
            finally
            {
                if (restoreInstanceTokenSources.TryRemove(regObjInfo, out var tokenSource))
                    tokenSource.Dispose();
            }
        }

        private bool TryRestoreInstanceFromMemoryAsyncCore(
            object obj,
            Type objType,
            object keyOrFactory,
            SceneInfo sceneInfo
            )
        {
            string resolvedKey = ResolveKey(keyOrFactory);
            var regObjInfo = new RegisteredObjectInfo(resolvedKey, objType, sceneInfo);

            if (!loadedSnapshots.TryGetValue(regObjInfo, out ISnapshot loadedSnapshot))
                return false;

            //Cancelling previous started task
            if (restoreInstanceTokenSources.TryRemove(regObjInfo, out var tokenSource))
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
            }

            if (!loadedSnapshot.CanRestore(obj))
                return false;

            if (obj is MonoBehaviour monoBeh)
            {
                tokenSource = destroyCancellationToken.LinkTokens(monoBeh.destroyCancellationToken);

                restoreInstanceTokenSources.TryAdd(regObjInfo, tokenSource);

                RestoreMonoBehaviourAfterInitialized(
                    regObjInfo,
                    monoBeh,
                    loadedSnapshot,
                    tokenSource.Token
                    )
                    .Forget();

                return true;
            }

            return loadedSnapshot.TryRestore(obj, out _);
        }

        private IDisposable RegisterObjectInternal(
            object obj,
            Type objType,
            object keyOrFactory,
            SceneInfo sceneInfo)
        {
            CC.Guard.IsNotNull(obj, nameof(obj));
            Guard.IsNotNull(objType, nameof(objType));
            CC.Guard.IsNotNull(keyOrFactory, nameof(keyOrFactory));

            if (!IsTypeRegistered(objType))
                throw new InvalidOperationException($"Type: {objType} is not registered");

            var regObj = new RegisteredObject(obj, sceneInfo, converters);

            if (IsInstanceRegisteredInternal(regObj))
                throw new InvalidOperationException(InstanceRegisterdMessage(obj));

            if (TryRestoreInstanceFromMemoryAsyncCore(
                obj,
                objType, 
                keyOrFactory,
                sceneInfo))
            {
                this.PrintLog($"Object: {obj} was restored by loaded snapshot");
            }

            objectKeys.Add(regObj, keyOrFactory);

            var sysObjects = objectSets.GetOrCreateNew(objType);

            sysObjects.Add(regObj);

            var disposables = sceneDisposables.GetOrCreateNew(sceneInfo);

            return new Registration(this, regObj).AddTo(disposables);
        }

        private string ResolveKey(object keyOrFactory)
        {
            return keyOrFactory switch
            {
                string key => key,
                IKeyFactory keyFactory => keyFactory.CreateKey().GetValue(() => throw new InvalidOperationException($"Cannot resolve key from input \"{keyOrFactory}\"")),
                _ => throw CC.ThrowHelper.InvalidOperationException(keyOrFactory, nameof(keyOrFactory)),
            };
        }

        private Maybe<string> ResolveKey(RegisteredObject regObj)
        {
            if (!objectKeys.TryGetValue(regObj, out object keyOrFactory))
                return Maybe<string>.None;

            return ResolveKey(keyOrFactory);
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

        private async UniTask<(SaveFileData fileData, string fileDataRaw)> LoadSaveFileAsync(string path, CancellationToken cancellationToken)
        {
            if (!File.Exists(path))
                return default;

            string serialized = await File.ReadAllTextAsync(path, cancellationToken);

            if (serialized.IsNullOrWhiteSpace())
                return default;

            var fileData = JsonConvert.DeserializeObject<SaveFileData>(serialized, CC.JsonSettings);

            return (fileData, serialized);
        }

        private async UniTask RegisterSnapshotsAsync(
            IList<SaveFileSceneData> snapshots, 
            CancellationToken cancellationToken)
        {
            loadedSnapshots.Clear();

            RegisteredObjectInfo regObjInfo;

            try
            {
                await UniTask.SwitchToThreadPool();

                foreach (var sceneData in snapshots)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    int iterationsPassed = 0;

                    foreach (var snapshot in sceneData.Snapshots)
                    {
                        cancellationToken.ThrowIfCancellationRequestedByIntervalAndMoveNext(ref iterationsPassed);

                        if (snapshot.Key is not string snapshotKey)
                        {
                            this.PrintError($"Key: {snapshot.Key} must be a string");
                            continue;
                        }

                        regObjInfo = new RegisteredObjectInfo(snapshotKey, snapshot.TargetType, sceneData.SceneInfo);
                        loadedSnapshots.TryAdd(regObjInfo, snapshot);

                        iterationsPassed++;
                    }
                }
            }
            finally
            {
                await UniTask.SwitchToMainThread();
            }
        }

        private void ApplyRegisteredSnapshots()
        {
            var loadedSceneInfos = SceneManagerHelper.GetLoadedSceneInfos().PrependToArray(default);

            foreach (var loadedSceneInfo in loadedSceneInfos)
            {
                foreach (var pair in loadedSnapshots)
                {
                    if (pair.Key.SceneInfo != loadedSceneInfo)
                        continue;

                    if (!pair.Value.TryRestore(target: null, out _))
                        continue;

                    loadedSnapshots.TryRemove(pair.Key, out _);
                }
            }
        }
    }
}
