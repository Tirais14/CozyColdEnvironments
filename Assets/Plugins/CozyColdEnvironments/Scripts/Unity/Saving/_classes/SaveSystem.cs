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
namespace CCEnvs.Unity.Saving
{
    public sealed class SaveSystem : CCBehaviourStaticPublic<SaveSystem>, ISaveSystem
    {
        private readonly Type gameObjectType = typeof(GameObject);

        private readonly Dictionary<Type, List<RegisteredObject>> objectLists = new();
        private readonly Dictionary<Type, Func<object, ISnapshot>> converters = new();
        private readonly Dictionary<RegisteredObject, string> objectKeys = new();
        private readonly Dictionary<SceneInfo?, List<IDisposable>> sceneDisposables = new();
        private UnityAction<Scene> onSceneUnloaded;

        protected override void Awake()
        {
            base.Awake();
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);

            onSceneUnloaded = scene =>
            {
                SceneInfo sceneInfo = scene.GetSceneInfo();

                if (!sceneDisposables.TryGetValue(sceneInfo, out var disposables))
                    return;

                for (int i = disposables.Count - 1; i >= 0; i--)
                {
                    disposables[i].Dispose();
                    disposables.RemoveAt(i);
                }
            };

            SceneManager.sceneUnloaded += onSceneUnloaded;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SceneManager.sceneUnloaded -= onSceneUnloaded;
            onSceneUnloaded = null!;
        }

        public IDisposable RegisterObject(object obj, string key, SceneInfo? sceneInfo = null)
        {
            CC.Guard.IsNotNull(obj, nameof(obj));

            Type objType = obj.GetType();
            return RegisterObjectInternal(obj, objType, key, sceneInfo: sceneInfo);
        }

        private IDisposable RegisterObjectInternal(
            object obj,
            Type objType,
            string key,
            SceneInfo? sceneInfo = null)
        {
            if (!obj.GetType().IsClass)
            {
                this.PrintError($"Cannot register {obj.GetType()}. It is not reference type");
                return Disposable.Empty;
            }

            sceneInfo ??= ResolveSceneInfo(obj);

            if (!IsTypeRegistered(objType))
            {
                this.PrintError($"Type: {objType} is not registered");
                return Disposable.Empty;
            }

            var sysObjects = objectLists.GetOrCreate(objType, () => new List<RegisteredObject>());
            var regObj = new RegisteredObject(obj, sceneInfo, converters);

            if (objectKeys.TryGetValue(regObj, out _) 
                ||
                sceneDisposables.TryGetValue(sceneInfo, out _))
            {
                this.PrintError($"Object key of object '{regObj}' already registered");
                return Disposable.Empty;
            }

            sysObjects.Add(regObj);
            objectKeys.Add(regObj, key);

            var disposables = sceneDisposables.GetOrCreate(sceneInfo, () => new List<IDisposable>());

            return Disposable.Create((@this: this, sysObjects, regObj),
                static input =>
                {
                    input.@this.UnregisterObjectInternal(input.sysObjects, input.regObj);
                })
                .AddTo(disposables);
        }

        public IDisposable RegisterGameObject(GameObject gameObject)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));
            return RegisterGameObjectInternal(gameObject, key: null, keyAsRuntimeId: true);
        }

        public IDisposable RegisterGameObject(GameObject gameObject, string key)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));
            Guard.IsNotWhiteSpace(key, nameof(key));
            return RegisterGameObjectInternal(gameObject, key, keyAsRuntimeId: false);
        }

        private IDisposable RegisterGameObjectInternal(
            GameObject gameObject,
            string? key,
            bool keyAsRuntimeId)
        {
            key ??= gameObject.GetHierarchyPath();
            if (keyAsRuntimeId)
            {
                if (!gameObject.TryGetComponent<RuntimeId>(out var idCmp))
                    gameObject.AddRuntimeIdComponent(key);
                else
                    key = idCmp.Id;
            }

            Scene goScene = gameObject.scene;
            SceneInfo goSceneInfo = goScene.GetSceneInfo();

            return RegisterObjectInternal(
                gameObject,
                gameObjectType,
                key,
                goSceneInfo);
        }

        public bool UnregisterObject(object? obj)
        {
            if (obj is null)
                return false;

            if (!objectLists.TryGetValue(obj.GetType(), out var sysObjects))
                return false;

            SceneInfo? objSceneInfo = ResolveSceneInfo(obj);
            RegisteredObject regObj = new RegisteredObject(obj, objSceneInfo, converters);

            return UnregisterObjectInternal(sysObjects, regObj);
        }

        private bool UnregisterObjectInternal(
            List<RegisteredObject> sysObjects,
            RegisteredObject regObj)
        {
            objectKeys.Remove(regObj);
            return sysObjects.Remove(regObj);
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

            converters.Add(type, converter);
        }
        public void RegisterType<T>(Func<T, ISnapshot> converter)
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
        public bool UnregisterType<T>()
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

        private readonly struct RegisteredObject : IEquatable<RegisteredObject>
        {
            private readonly IDictionary<Type, Func<object, ISnapshot>> converters;

            public object Object { get; }
            public Type ObjectType { get; }
            public SceneInfo? SceneInfo { get; }

            public RegisteredObject(
                object obj,
                SceneInfo? sceneInfo,
                IDictionary<Type, Func<object, ISnapshot>> converters)
            {
                Guard.IsNotNull(obj, nameof(obj));
                CC.Guard.IsNotNull(converters, nameof(converters));

                Object = obj;
                SceneInfo = sceneInfo;
                this.converters = converters;

                ObjectType = obj.GetType();
            }

            public void Deconstruct(out object obj, out Type objType, out SceneInfo? sceneInfo)
            {
                obj = Object;
                objType = ObjectType;
                sceneInfo = SceneInfo;
            }

            public static bool operator ==(RegisteredObject left, RegisteredObject right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(RegisteredObject left, RegisteredObject right)
            {
                return !(left == right);
            }

            public ISnapshot ConvertToSnapshot()
            {
                if (!converters.TryGetValue(ObjectType, out var converter))
                    throw new InvalidOperationException($"Registration of type '{ObjectType}' invalid");

                return converter(Object);
            }

            public override bool Equals(object? obj)
            {
                return obj is RegisteredObject @object && Equals(@object);
            }

            public bool Equals(RegisteredObject other)
            {
                return EqualityComparer<object>.Default.Equals(Object, other.Object)
                       &&
                       ObjectType.Equals(other.ObjectType)
                       &&
                       SceneInfo.Equals(other.SceneInfo)
                       &&
                       converters.Equals(other.converters);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Object, ObjectType, SceneInfo, converters);
            }
        }
    }

    public static class SaveSystemExtensions
    {
        public static IDisposable RegisterObjectToSaveSystem(
            this object source,
            string key,
            SceneInfo? sceneInfo = null)
        {
            return SaveSystem.Self.RegisterObject(source, key, sceneInfo: sceneInfo);
        }

        public static IDisposable RegisterGameObject(this GameObject source)
        {
            return SaveSystem.Self.RegisterGameObject(source);
        }

        public static IDisposable RegisterGameObject(this GameObject source, string key)
        {
            return SaveSystem.Self.RegisterGameObject(source, key);
        }
    }
}
