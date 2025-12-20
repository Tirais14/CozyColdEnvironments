using CCEnvs.Collections;
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
namespace CCEnvs.Unity.Saving
{
    public sealed partial class SavingSystem : CCBehaviourStaticPublic<SavingSystem>, ISavingSystem
    {
        private readonly Type gameObjectType = typeof(GameObject);

        private readonly Dictionary<Type, HashSet<RegisteredObject>> objectLists = new();

        private readonly Dictionary<RegisteredObject, object> objectKeys = new();

        private readonly Dictionary<Type, Func<object, ISnapshot>> converters = new();
        private readonly Dictionary<SceneInfo?, CompositeDisposable> sceneDisposables = new();

        private UnityAction<Scene> onSceneUnloaded;

        protected override void Awake()
        {
            base.Awake();
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);

            onSceneUnloaded = scene =>
            {
                SceneInfo sceneInfo = scene.GetSceneInfo();

                if (!sceneDisposables.Remove(sceneInfo, out var disposables))
                    return;

                disposables.Dispose();
            };

            SceneManager.sceneUnloaded += onSceneUnloaded;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SceneManager.sceneUnloaded -= onSceneUnloaded;
            onSceneUnloaded = null!;
        }

        private static string InstanceRegisterdMessage(object obj)
        {
            return $"Instance '{obj}' already registered";
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

            object key = ResolveKeyForUnityObject(component, key: null);
            SceneInfo sceneInfo = component.gameObject.scene.GetSceneInfo();

            return RegisterObjectInternal(component, component.GetType(), key, sceneInfo);
        }

        public IDisposable RegisterUnityObject(GameObject gameObject)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));

            object key = ResolveKeyForUnityObject(gameObject, key: null);
            SceneInfo sceneInfo = gameObject.scene.GetSceneInfo();

            return RegisterObjectInternal(gameObject, gameObjectType, key, sceneInfo);
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
            await UniTask.SwitchToThreadPool();
            string serialized = await File.ReadAllTextAsync(path);
            await UniTask.SwitchToMainThread();

            var saveFileData = JsonConvert.DeserializeObject<SaveFileData>(serialized, CC.JsonOptions);
            saveFileData.ApplyToLoadedScenes();
        }

        public async UniTask SaveAsync(string path)
        {
            SaveFileData saveFileData = await BuildSaveFileDataAsync();
            string serialized = JsonConvert.SerializeObject(saveFileData, CC.JsonOptions);

            await UniTask.SwitchToThreadPool();
            await File.WriteAllTextAsync(path, serialized);
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
            return objectKeys.ContainsKey(regObj) || objectLists[regObj.ObjectType].Contains(regObj);
        }

        private SceneInfo? ResolveSceneInfo(object obj)
        {
            if (obj is not UnityEngine.Object uObject)
                return null;

            Scene objScene = GameObject.GetScene(uObject.GetInstanceID());

            if (!objScene.IsValid())
                return null;

            return objScene.GetSceneInfo();
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
            using var _ = ListPool<SaveFileSceneData>.Get(out var sceneDatas);
            using var __ = ListPool<ISnapshot>.Get(out var snapshots);

            ISnapshot snapshot;
            SaveFileSceneData sceneData;
            foreach (var sceneInfo in SceneManagerHelper.GetLoadedScenes().Select(x => x.GetSceneInfo()))
            {
                using PooledArray<RegisteredObject> regObjects = await GetSceneObjectsAsync(sceneInfo);
                foreach (var regObj in regObjects.Value)
                {
                    try
                    {
                        snapshot = regObj.ConvertToSnapshot();
                        snapshots.Add(snapshot);
                    }
                    catch (Exception ex)
                    {
                        this.PrintException(ex);
                    }
                }

                sceneData = new SaveFileSceneData(sceneInfo, snapshots);
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

        private object ResolveKeyForUnityObject(UnityEngine.Object uObject, string? key)
        {
            switch (uObject)
            {
                case GameObject go:
                    {
                        return new KeyFactory<GameObject, string?>(go, key,
                            static (go, key) =>
                            {
                                bool keyValid = key.IsNotNullOrWhiteSpace();

                                if (go.TryGetComponent<RuntimeId>(out var idCmp))
                                {
                                    if (idCmp.Id.IsNullOrWhiteSpace())
                                        return go.GetHierarchyPath();

                                    return idCmp.Id;
                                }
                                else if (keyValid && idCmp.IsNull())
                                {
                                    go.AddRuntimeIdComponent(key!);
                                    return key!;
                                }

                                return go.GetHierarchyPath();
                            });
                    }
                case Component cmp:
                    return ResolveKeyForUnityObject(cmp.gameObject, key);
                default:
                    throw CC.ThrowHelper.InvalidOperationException(uObject.GetType());
            }
        }

        private IDisposable RegisterObjectInternal(
            object obj,
            Type objType,
            object keyUntyped,
            SceneInfo? sceneInfo = null)
        {
            CC.Guard.IsNotNull(obj, nameof(obj));
            Guard.IsNotNull(objType, nameof(objType));
            CC.Guard.IsNotNull(keyUntyped, nameof(keyUntyped));

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

            objectKeys.Add(regObj, keyUntyped);

            var sysObjects = objectLists.GetOrCreateNew(objType);

            sysObjects.Add(regObj);

            var disposables = sceneDisposables.GetOrCreateNew(sceneInfo);

            return new Registration(this, regObj).AddTo(disposables);
        }

        private void RegisterKey(RegisteredObject regObj, object keyUntyped)
        {
            switch (keyUntyped)
            {
                case string key:
                    objectKeys.Add(regObj, key);
                    break;
                case IKeyFactory keyFactory:
                    objectKeys.Add(regObj, keyFactory);
                    break;
                default:
                    break;
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
