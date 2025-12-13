using CCEnvs.Diagnostics;
using CCEnvs.Snapshots;
using CCEnvs.Unity.Components;
using CommunityToolkit.Diagnostics;
using ConcurrentCollections;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UniRx;
using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.Unity.Saving
{
    public sealed class SaveSystem : CCBehaviourStaticPublic<SaveSystem>, ISaveSystem
    {
        private readonly Dictionary<Type, ConcurrentHashSet<object>> collections = new();
        private readonly Dictionary<Type, Func<object, ISnapshot>> converters = new();
        private readonly List<SerializedSnapshotInfo> toSerilizeInfos = new();

        protected override void Awake()
        {
            base.Awake();
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            foreach (var collection in collections.Values)
                collection.Clear();

            collections.Clear();
        }

        public IDisposable Register(object obj)
        {
            CC.Guard.IsNotNull(obj, nameof(obj));

            if (!collections.TryGetValue(obj.GetType(), out var collection))
            {
                collection = new ConcurrentHashSet<object>();
                collections.TryAdd(obj.GetType(), collection);
            }

            collection.Add(obj);

            return Disposable.CreateWithState((@this: this, obj),
                static input =>
                {
                    input.@this.Unregister(input.obj);
                })
                .AddTo(this);
        }

        public bool Unregister(object? obj)
        {
            if (obj is null)
                return false;

            if (!collections.TryGetValue(obj.GetType(), out var collection))
                return false;

            return collection.TryRemove(obj);
        }

        public void Load(string path)
        {
            var t = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "alcoSave.json");
            string serailized = File.ReadAllText(t);

            ISnapshot[] snapshots = Deserialize(serailized);
            snapshots.RestoreStates();
        }

        public void Save(string path)
        {
            SerializeObjects();
            string serialized = JsonConvert.SerializeObject(toSerilizeInfos, Formatting.Indented);

            var t = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "alcoSave.json");
            File.WriteAllText(t, serialized);
        }

        public void RegisterType(Type type, Func<object, ISnapshot> converter)
        {
            Guard.IsNotNull(type);
            Guard.IsNotNull(converter);

            if (IsTypeRegistered(type))
                throw new InvalidOperationException($"Type: {type} already registered.");

            collections.Add(type, new ConcurrentHashSet<object>());

            if (converter is not null)
                converters.Add(type, converter);
        }

        public bool UnregisterType(Type? type)
        {
            if (type is null)
                return false;

            converters.Remove(type);
            return collections.Remove(type);
        }

        public bool IsTypeRegistered(Type? type)
        {
            if (type is null)
                return false;

            return collections.ContainsKey(type);
        }

        //private async UniTask Execute()
        //{
        //    await UniTask.SwitchToThreadPool();

        //    var toRemoveList = new List<(Type, object)>();
        //    while (!destroyCancellationToken.IsCancellationRequested)
        //    {
        //        toRemoveList.Clear();
        //        await UniTask.WaitForEndOfFrame();

        //        foreach (var pair in collections)
        //        {
        //            foreach (var obj in pair.Value)
        //            {
        //                if (obj.IsNull())
        //                    toRemoveList.Add((pair.Key, obj));
        //            }
        //        }

        //        (Type, object) toRemovePair;
        //        for (int i = 0; i < toRemoveList.Count; i++)
        //        {
        //            toRemovePair = toRemoveList[i];

        //            if (collections.TryGetValue(toRemovePair.Item1, out var collection))
        //                collection.TryRemove(toRemovePair.Item2);
        //        }
        //    }
        //}

        private void SerializeObjects()
        {
            toSerilizeInfos.Clear();
            ISnapshot converted;
            foreach (var pair in collections)
            {
                foreach (var obj in pair.Value)
                {
                    Func<object, ISnapshot> converter = converters[pair.Key];
                    converted = converter(obj);
                    try
                    {
                        SerializedSnapshotInfo serialized = SerilializeSnapshot(converted);
                        toSerilizeInfos.Add(serialized);
                    }
                    catch (Exception ex)
                    {
                        this.PrintException(ex);
                    }
                }
            }
        }

        private SerializedSnapshotInfo SerilializeSnapshot(ISnapshot converted)
        {
            return new SerializedSnapshotInfo(converted);
        }

        private ISnapshot[] Deserialize(string serialized)
        {
            var pairs = JsonConvert.DeserializeObject<List<SerializedSnapshotInfo>>(serialized);
            return pairs.Select(pair => pair.Deserialize()).ToArray();
        }
    }

    public static class SaveSystemExtensions
    {
        public static IDisposable RegisterForSaving(this object source)
        {
            CC.Guard.IsNotNull(source, nameof(source));
            return SaveSystem.Self.Register(source);
        }
    }
}
