using CCEnvs.Linq;
using CCEnvs.Snapshots;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public readonly struct SaveGroup : IEquatable<SaveGroup>
    {
        private readonly LazyLight<ConcurrentDictionary<string, object>> observableObjects;

        public string Name { get; }
        public string? ID { get; }

        public SaveGroup(
            string name,
            string? id = null
            )
        {
            observableObjects = new LazyLight<ConcurrentDictionary<string, object>>(
                static () => new ConcurrentDictionary<string, object>()
                );

            Name = name;
            ID = id;
        }

        public static bool operator ==(SaveGroup left, SaveGroup right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SaveGroup left, SaveGroup right)
        {
            return !(left == right);
        }

        public readonly void RegisterObject(object obj, string? key = null)
        {
            CC.Guard.IsNotNull(obj, nameof(obj));

            if (key is null && !TryResolveKey(obj, out key))
                key = string.Empty;

            if (!observableObjects.HasValue || observableObjects.Value.ContainsKey(key))
                throw new ArgumentException($"Object: {obj} with key: {key} already registered");

            observableObjects.Value.TryAdd(key, obj);
        }

        public readonly bool UnregisterObject(string key)
        {
            Guard.IsNotNull(key, nameof(key));

            return observableObjects.HasValue && observableObjects.Value.TryRemove(key, out _);
        }

        public readonly bool UnregisterObject(object obj)
        {
            if (!TryResolveKey(obj, out var key))
                key = string.Empty;

            return UnregisterObject(key);
        }

        public readonly bool IsObjectRegistered(string? key)
        {
            if (key is null)
                return false;

            return observableObjects.HasValue && observableObjects.Value.ContainsKey(key);
        }

        public readonly bool IsObjectRegistered(object obj)
        {
            if (!TryResolveKey(obj, out var key))
                key = string.Empty;

            return IsObjectRegistered(key);
        }

        public readonly bool TryResolveKey(
            object obj, 
            [NotNullWhen(true)] out string? key
            )
        {
            CC.Guard.IsNotNull(obj, nameof(obj));

            key = obj switch
            {
                GameObject go => ResolveGameObjectKey(go),
                Component cmp => ResolveComponentKey(cmp),
                _ => null
            };

            return key is not null;
        }

        public readonly SaveData GetSaveData()
        {
            try
            {
                var objConverterPairs = GetObjectConverterPairs();

                return SaveDataFactory.Create(objConverterPairs);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);

                return default;
            }
        }

        private readonly string ResolveGameObjectKey(GameObject go)
        {
            return go.GetExtraInfo().ToString();
        }

        private readonly string ResolveComponentKey(Component cmp)
        {
            return cmp.GetExtraInfo().ToString();
        }

        private IEnumerable<(object obj, string key, Func<object, ISnapshot> converter)> GetObjectConverterPairs()
        {
            if (!observableObjects.HasValue)
                return Array.Empty<(object obj, string key, Func<object, ISnapshot> converter)>();

            return observableObjects.Value.Select(
                static pair =>
                {
                    var objType = pair.Value.GetType();

                    return (pair.Value, pair.Key, SaveSystem.GetSnapshotConverter(objType));
                });
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is SaveGroup group && Equals(group);
        }

        public readonly bool Equals(SaveGroup other)
        {
            return observableObjects.Equals(other.observableObjects)
                   &&
                   Name == other.Name
                   &&
                   ID == other.ID;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(observableObjects, Name, ID);
        }
    }
}
