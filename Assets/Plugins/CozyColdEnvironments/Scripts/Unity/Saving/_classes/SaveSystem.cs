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
                throw new InvalidOperationException($"Type: {objType} is not registered");

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

        private async UniTask<PooledArray<(object obj, Func<object, ISnapshot> converter)>> GetSceneObjectsAsync(SceneInfo sceneInfo)
        {
            await UniTask.SwitchToThreadPool();

            using var _ = ListPool<(object obj, Func<object, ISnapshot> converter)>.Get(out var results);

            (object obj, Func<object, ISnapshot> converter) sceneObj;
            foreach (var (obj, objSceneInfo) in objSets.Values.SelectMany(x => x))
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
