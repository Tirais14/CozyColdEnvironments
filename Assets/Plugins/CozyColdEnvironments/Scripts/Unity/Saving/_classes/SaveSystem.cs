using CCEnvs.Pools;
using CCEnvs.Snapshots;
using CCEnvs.Unity.Components;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using CCEnvs.TypeMatching;
using CCEnvs.FuncLanguage;

#nullable enable
namespace CCEnvs.Unity.Saving
{
    public sealed class SaveSystem : CCBehaviourStaticPublic<SaveSystem>, ISaveSystem
    {
        private readonly Type gameObjectType = typeof(GameObject);

        private readonly Dictionary<Type, HashSet<SaveSystemObject>> objLists = new();
        private readonly Dictionary<Type, Func<object, ISnapshot>> converters = new();
        private readonly Dictionary<SnapshotKey, ISnapshot>

        protected override void Awake()
        {
            base.Awake();
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var collection in objLists.Values)
                collection.Clear();

            objLists.Clear();
        }

        public IDisposable RegisterObject(object obj, string key, SceneInfo? sceneInfo = null)
        {
            CC.Guard.IsNotNull(obj, nameof(obj));

            Type objType  = obj.GetType();
            if (!obj.GetType().IsClass)
            {
                this.PrintError($"Cannot register {obj.GetType()}. It is not reference type");
                return Disposable.Empty;
            }

            if (obj is GameObject)
            {
                this.PrintError($"Object was't registered. Use specially {nameof(RegisterGameObject)} method to register GameObject");
                return Disposable.Empty;
            }
            sceneInfo ??= ResolveSceneInfo(obj);

            return RegisterObjectInternal(obj, objType, key, sceneInfo: sceneInfo);
        }

        public IDisposable RegisterObject<TObject, TState>(
            TObject obj,
            Func<TObject, Maybe<TState>, string> keySelector,
            SceneInfo? sceneInfo = null,
            TState? state = default) where TObject : class
        {
            Guard.IsNotNull(obj, nameof(obj));
            Guard.IsNotNull(keySelector, nameof(keySelector));

            return RegisterObject(obj, keySelector(obj, state.Maybe()), sceneInfo: sceneInfo);
        }

        private IDisposable RegisterObjectInternal(
            object obj,
            Type objType,
            string key,
            SceneInfo? sceneInfo = null)
        {
            if (obj is GameObject)
            {
                this.PrintError($"Object was't registered. Use specially {nameof(RegisterGameObject)} method to register GameObject");
                return Disposable.Empty;
            }
            sceneInfo ??= ResolveSceneInfo(obj);

            if (!IsTypeRegistered(objType))
                throw new InvalidOperationException($"Type: {objType} is not registered");

            if (!objLists.TryGetValue(objType, out var objs))
            {
                objs = new HashSet<SaveSystemObject>();
                objLists.Add(objType, objs);
            }

            var saveSysObj = new SaveSystemObject(obj, sceneInfo.Value, key, converters);
            objs.Add((obj, sceneInfo.Value));
            objScenes.Add((obj, obj.GetType()), sceneInfo.Value);

            return Disposable.Create((@this: this, obj),
                static input =>
                {
                    input.@this.UnregisterObject(input.obj);
                })
                .BindDisposableTo(this);
        }

        public IDisposable RegisterGameObject(GameObject gameObject, string runtimeId)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));
            Guard.IsNotNullOrWhiteSpace(runtimeId, nameof(runtimeId));

            Type runtimeIdType = typeof(RuntimeId);

            if (!gameObject.GetComponents<Component>()
                .FirstOrDefault(cmp => cmp.GetType() == runtimeIdType)
                .Is<RuntimeId>(out var runtimeIdCmp))
            {
                runtimeIdCmp = gameObject.AddComponent<RuntimeId>();
            }

            runtimeIdCmp.SetId(runtimeId);
            SceneInfo sceneInfo = GameObject.GetScene(gameObject.GetInstanceID()).GetSceneInfo();
            return RegisterObjectInternal(gameObject, gameObjectType, runtimeId, sceneInfo: sceneInfo);
        }

        public IDisposable RegisterGameObject(GameObject gameObject)
        {
            throw new NotImplementedException();
        }

        public IDisposable RegisterGameObject<TState>(GameObject gameObject, Func<GameObject, Maybe<TState>, string> runtimeIdSelector, TState? state = default)
        {
            throw new NotImplementedException();
        }

        public IDisposable RegisterGameObject(
            GameObject gameObject,
            Func<GameObject, string> runtimeIdSelector)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));
            Guard.IsNotNull(runtimeIdSelector, nameof(runtimeIdSelector));
            return RegisterGameObject(gameObject, runtimeIdSelector(gameObject));
        }

        public bool UnregisterObject(object? obj)
        {
            if (obj is null)
                return false;

            if (!objLists.TryGetValue(obj.GetType(), out var collection))
                return false;

            objScenes.Remove((obj, obj.GetType()), out SceneInfo sceneInfo);
            return collection.Remove((obj, sceneInfo));
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
            Guard.IsNotNull(type);
            Guard.IsNotNull(converter);

            if (IsTypeRegistered(type))
                throw new InvalidOperationException($"Type: {type} already registered.");

            registeredTypes.Add(type);

            if (converter is not null)
                converters.Add(type, converter);
        }
        public void RegisterType<T>(Func<object, ISnapshot> converter)
        {
            RegisterType(typeof(T), converter);
        }

        public bool UnregisterType(Type? type)
        {
            if (type is null)
                return false;

            converters.Remove(type);

            foreach (var key in objLists.Keys)
                objLists.Remove(key);

            return registeredTypes.Remove(type);
        }
        public bool UnregisterType<T>()
        {
            return UnregisterType(typeof(T));
        }

        public bool IsTypeRegistered(Type? type)
        {
            if (type is null)
                return false;

            return registeredTypes.Contains(type);
        }
        public bool IsTypeRegistered<T>()
        {
            return IsTypeRegistered(typeof(T));
        }

        private SceneInfo ResolveSceneInfo(object obj)
        {
            if (obj is UnityEngine.Object uObject)
            {
                Scene objScene = GameObject.GetScene(uObject.GetInstanceID());

                if (objScene.IsValid())
                    return objScene.GetSceneInfo();
            }

            return SceneManager.GetActiveScene().GetSceneInfo();
        }

        private async UniTask<PooledArray<(object obj, Func<object, ISnapshot> converter)>> GetSceneObjectsAsync(SceneInfo sceneInfo)
        {
            await UniTask.SwitchToThreadPool();

            using var _ = ListPool<(object obj, Func<object, ISnapshot> converter)>.Get(out var results);

            (object obj, Func<object, ISnapshot> converter) sceneObj;
            foreach (var (obj, objSceneInfo) in objLists.Values.SelectMany(x => x))
            {
                if (objSceneInfo != sceneInfo)
                    continue;

                sceneObj = (obj, converters[obj.GetType()]);
                results.Add(sceneObj);
            }

            await UniTask.SwitchToMainThread();
            return results.ToArrayPooled();
        }

        private async UniTask<PooledArray<SaveSceneData>> BuildSceneDatasAsync()
        {
            using var _ = ListPool<SaveSceneData>.Get(out var sceneDatas);
            using var __ = ListPool<ISnapshot>.Get(out var snapshots);

            ISnapshot snapshot;
            SaveSceneData sceneData;
            foreach (var sceneInfo in SceneManagerHelper.GetLoadedScenes().Select(x => x.GetSceneInfo()))
            {
                using var sceneObjects = await GetSceneObjectsAsync(sceneInfo);
                foreach (var (obj, converter) in sceneObjects.Value)
                {
                    try
                    {
                        snapshot = converter(obj);
                        snapshots.Add(snapshot);
                    }
                    catch (Exception ex)
                    {
                        this.PrintException(ex);
                    }
                }

                sceneData = new SaveSceneData(sceneInfo, snapshots);
                sceneDatas.Add(sceneData);
                snapshots.Clear();
            }

            return sceneDatas.ToArrayPooled();
        }

        private async UniTask<SaveFileData> BuildSaveFileDataAsync()
        {
            using PooledArray<SaveSceneData> sceneDatas = await BuildSceneDatasAsync();
            return new SaveFileData("0.0.0.0", sceneDatas.Value.ToImmutableArray()); //TODO: Versioning
        }

        private readonly struct SnapshotKey : IEquatable<SnapshotKey>
        {
            public object Object { get; } 
            public string Key { get; }

            public Type ObjectType { get; }

            public SnapshotKey(object obj, string key)
            {
                Object = obj;
                Key = key;

                ObjectType = Object.GetType();
            }

            public static bool operator ==(SnapshotKey left, SnapshotKey right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(SnapshotKey left, SnapshotKey right)
            {
                return !(left == right);
            }

            public override bool Equals(object? obj)
            {
                return obj is SnapshotKey key && Equals(key);
            }

            public bool Equals(SnapshotKey other)
            {
                return System.Collections.Generic.EqualityComparer<object>.Default.Equals(Object, other.Object)
                       &&
                       Key == other.Key
                       &&
                       ObjectType == other.ObjectType;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Object, Key, ObjectType);
            }
        }
    }

    public static class SaveSystemExtensions
    {
        /// <inheritdoc cref=" SaveSystem.BindObject(object, SceneInfo?)"/>
        public static IDisposable BindToSaveSystem(this object source)
        {
            return SaveSystem.Self.BindObject(source);
        }

        /// <inheritdoc cref=" SaveSystem.RegisterGameObject(GameObject, string)"/>
        public static IDisposable BindGameObjectToSaveSystem(
            this GameObject source, 
            string runtimeId)
        {
            return SaveSystem.Self.RegisterGameObject(source, runtimeId);
        }

        /// <inheritdoc cref=" SaveSystem.RegisterGameObject(GameObject, Func{GameObject, string})"/>
        public static IDisposable BindGameObjectToSaveSystem(
            this GameObject source,
            Func<GameObject, string> runtimeIdSelector)
        {
            return SaveSystem.Self.RegisterGameObject(source, runtimeIdSelector);
        }

        public static bool IsTypeRegisteredInSaveSystem(this Type? source)
        {
            return SaveSystem.Self.IsTypeRegistered(source);
        }

        public static bool IsTypeRegisteredInSaveSystem(this object source)
        {
            return SaveSystem.Self.IsTypeRegistered(source.GetType());
        }
    }
}
