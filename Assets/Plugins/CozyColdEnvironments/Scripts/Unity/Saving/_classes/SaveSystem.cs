using CCEnvs.Snapshots;
using CCEnvs.Unity.Components;
using CommunityToolkit.Diagnostics;
using ConcurrentCollections;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using R3;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using CCEnvs.Diagnostics;

#nullable enable
namespace CCEnvs.Unity.Saving
{
    public sealed class SaveSystem : CCBehaviourStaticPublic<SaveSystem>, ISaveSystem
    {
        private readonly Dictionary<(Type type, SceneInfo sceneInfo), ConcurrentHashSet<object>> collections = new();
        private readonly Dictionary<Type, Func<object, ISnapshot>> converters = new();
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

            foreach (var collection in collections.Values)
                collection.Clear();

            collections.Clear();
        }

        public IDisposable BindObject(object obj, SceneInfo sceneInfo)
        {
            CC.Guard.IsNotNull(obj, nameof(obj));

            Type objType = obj.GetType();

            if (!IsTypeRegistered(objType))
                throw new InvalidOperationException($"Type: {objType.GetType()} is not registered");

            if (!collections.TryGetValue((objType, sceneInfo), out var collection))
            {
                collection = new ConcurrentHashSet<object>();
                collections.TryAdd((objType, sceneInfo), collection);
            }

            collection.Add(obj);

            return Disposable.Create((@this: this, obj, sceneInfo),
                static input =>
                {
                    input.@this.UnbindObject(input.obj, input.sceneInfo);
                })
                .BindDisposableTo(this);
        }

        public IDisposable BindObject(object obj, Scene scene)
        {
            return BindObject(obj, new SceneInfo(scene));
        }

        public bool UnbindObject(object? obj, SceneInfo sceneInfo)
        {
            if (obj is null)
                return false;

            if (!collections.TryGetValue((obj.GetType(), sceneInfo), out var collection))
                return false;

            return collection.TryRemove(obj);
        }

        public bool UnbindObject(object? obj, Scene scene)
        {
            return UnbindObject(obj, new SceneInfo(scene));
        }

        public async UniTask LoadAsync(string path)
        {
            string serailized = File.ReadAllText(path);
            ISnapshot[] snapshots = Deserialize(serailized);
            snapshots.RestoreStates();
        }

        public async UniTask SaveAsync(string path)
        {
            SaveContext[] saveContexts = SerializeObjects();
            string serialized = JsonConvert.SerializeObject(saveContexts, Formatting.Indented);
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

            foreach (var key in collections.Keys.Where(x => x.type == type))
                collections.Remove(key);

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

        private SaveContext[] SerializeObjects()
        {
            Dictionary<SceneInfo, List<string>> rawData = new();
            ISnapshot snapshot;
            foreach (var pair in collections)
            {
                foreach (var obj in pair.Value)
                {
                    if (!rawData.TryGetValue(pair.Key.sceneInfo, out var serializedSnapshots))
                        serializedSnapshots = new List<string>();

                    try
                    {
                        Func<object, ISnapshot> converter = converters[pair.Key.type];
                        snapshot = converter(obj);

                        var serialized = JsonConvert.SerializeObject(snapshot);
                        serializedSnapshots.Add(serialized);
                    }
                    catch (Exception ex)
                    {
                        this.PrintException(ex);
                    }
                }
            }

            using var _ = ListPool<SaveContext>.Get(out var contexts);

            SaveContext saveContext;
            foreach (var item in rawData)
            {
                saveContext = new SaveContext(item.Key, item.Value.ToImmutableArray());
                contexts.Add(saveContext);
            }

            return contexts.ToArray();
        }

        private SaveUnit SerilializeSnapshot(ISnapshot converted)
        {
            return new SaveUnit(converted);
        }

        private ISnapshot[] Deserialize(string serialized)
        {
            var contexts = JsonConvert.DeserializeObject<SaveContext[]>(serialized);

            return contexts.SelectMany(ctx => ctx.Data)
                           .Select(content => JsonConvert.DeserializeObject<ISnapshot>(content)!)
                           .Where(x => x.IsNotNull())
                           .ToArray();
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
            return SaveSystem.Self.BindObject(source, SceneManager.GetActiveScene());
        }

        public static IDisposable BindToSaveSystem(this object source, SceneInfo scene)
        {
            CC.Guard.IsNotNull(source, nameof(source));
            return SaveSystem.Self.BindObject(source, scene);
        }

        public static IDisposable BindToSaveSystem(this object source, Scene scene)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            if (!scene.IsValid())
                throw new ArgumentException($"{nameof(scene)} is not valid");

            return SaveSystem.Self.BindObject(source, scene);
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
