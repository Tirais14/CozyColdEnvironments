using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Json;
using CCEnvs.Json.Converters;
using CCEnvs.Pools;
using CCEnvs.Snapshots;
using CCEnvs.Unity.Components;
using CommunityToolkit.Diagnostics;
using ConcurrentCollections;
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.Unity.Saving
{
    public sealed class SaveSystem : CCBehaviourStaticPublic<SaveSystem>, ISaveSystem
    {
        private readonly Dictionary<Type, HashSet<(object obj, SceneInfo sceneInfo)>> objSets = new();
        private readonly Dictionary<Type, Func<object, ISnapshot>> converters = new();
        private readonly Dictionary<(object obj, Type objType), SceneInfo> objScenes = new();
        private readonly HashSet<Type> registeredTypes = new();

        protected override void Awake()
        {
            base.Awake();
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var collection in objSets.Values)
                collection.Clear();

            objSets.Clear();
        }

        public IDisposable BindObject(object obj, SceneInfo? sceneInfo = null)
        {
            CC.Guard.IsNotNull(obj, nameof(obj));

            Type objType = obj.GetType();
            sceneInfo ??= ResolveSceneInfo(obj);

            if (!IsTypeRegistered(objType))
                throw new InvalidOperationException($"Type: {objType.GetType()} is not registered");

            if (!objSets.TryGetValue(objType, out var objs))
            {
                objs = new HashSet<(object obj, SceneInfo sceneInfo)>();
                objSets.Add(objType, objs);
            }

            objs.Add((obj, sceneInfo.Value));
            objScenes.Add((obj, obj.GetType()), sceneInfo.Value);

            return Disposable.Create((@this: this, obj),
                static input =>
                {
                    input.@this.UnbindObject(input.obj);
                })
                .BindDisposableTo(this);
        }

        public bool UnbindObject(object? obj)
        {
            if (obj is null)
                return false;

            if (!objSets.TryGetValue(obj.GetType(), out var collection))
                return false;

            objScenes.Remove((obj, obj.GetType()), out SceneInfo sceneInfo);
            return collection.Remove((obj, sceneInfo));
        }

        public async UniTask LoadAsync(string path)
        {
            string serailized = File.ReadAllText(path);
            var ctx = JsonSerializer.Deserialize<SaveFileData>(serailized, CC.JsonOptions);
            ctx.ApplyToLoadedScenes();
        }

        public async UniTask SaveAsync(string path)
        {
            SaveFileData saveFileData = BuildSaveFileData();
            string serialized = JsonSerializer.Serialize(saveFileData, CC.JsonOptions);
            File.WriteAllText(path, serialized);
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

            foreach (var key in objSets.Keys)
                objSets.Remove(key);

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

        private PooledArray<(object obj, Func<object, ISnapshot> converter)> GetSceneObjects(SceneInfo sceneInfo)
        {
            using var _ = ListPool<(object obj, Func<object, ISnapshot> converter)>.Get(out var results);

            (object obj, Func<object, ISnapshot> converter) sceneObj;
            foreach (var (obj, objSceneInfo) in objSets.Values.SelectMany(x => x))
            {
                if (objSceneInfo != sceneInfo)
                    continue;

                sceneObj = (obj, converters[obj.GetType()]);
                results.Add(sceneObj);
            }

            return results.ToArrayPooled();
        }

        private PooledArray<SaveSceneData> BuildSceneDatas()
        {
            using var _ = ListPool<SaveSceneData>.Get(out var sceneDatas);
            using var __ = ListPool<ISnapshot>.Get(out var snapshots);

            ISnapshot snapshot;
            SaveSceneData sceneData;
            foreach (var sceneInfo in SceneManagerHelper.GetLoadedScenes().Select(x => x.GetSceneInfo()))
            {
                using var sceneObjects = GetSceneObjects(sceneInfo);
                foreach (var (obj, converter) in sceneObjects.Value)
                {
                    snapshot = converter(obj);
                    snapshots.Add(snapshot);
                }

                sceneData = new SaveSceneData(sceneInfo, snapshots);
                sceneDatas.Add(sceneData);
                snapshots.Clear();
            }

            return sceneDatas.ToArrayPooled();
        }

        private SaveFileData BuildSaveFileData()
        {
            using PooledArray<SaveSceneData> sceneDatas = BuildSceneDatas();
            return new SaveFileData(sceneDatas.Value, "0.0.0.0"); //TODO: Versioning
        }
    }

    public static class SaveSystemExtensions
    {
        /// <summary>
        /// Use <see cref="SceneManager.GetActiveScene()"/> for scene info argument
        /// </summary>
        public static IDisposable BindToSaveSystem(this object source)
        {
            CC.Guard.IsNotNull(source, nameof(source));
            return SaveSystem.Self.BindObject(source);
        }

        /// <summary>
        /// Binds only registered types
        /// </summary>
        public static IDisposable[] BindComponentsToSaveSystem(this GameObject source)
        {
            CC.Guard.IsNotNull(source, nameof(source));
            using var _ = ListPool<IDisposable>.Get(out var list);
            foreach (var cmp in source.GetComponents<Component>())
            {
                if (cmp.IsTypeRegisteredInSaveSystem())
                    list.Add(cmp.BindToSaveSystem());
            }

            return list.ToArray();
        }

        public static bool IsTypeRegisteredInSaveSystem(this Type? source)
        {
            return SaveSystem.Self.IsTypeRegistered(source);
        }

        public static bool IsTypeRegisteredInSaveSystem(this object source)
        {
            Guard.IsNotNull(source);
            return SaveSystem.Self.IsTypeRegistered(source.GetType());
        }
    }
}
