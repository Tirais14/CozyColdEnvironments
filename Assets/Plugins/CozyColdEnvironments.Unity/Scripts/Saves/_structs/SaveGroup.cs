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
    public class SaveGroup
    {
        private readonly ConcurrentDictionary<string, object> observableObjects = new();

        public string Name { get; }
        public string? ID { get; }

        public SaveGroup(
            string name,
            string? id = null
            )
        {
            Name = name;
            ID = id;
        }

        public bool TryRegisterObject(object obj, string? key = null)
        {
            CC.Guard.IsNotNull(obj, nameof(obj));

            if (key is null && !TryResolveKey(obj, out key))
                key = string.Empty;

            if (!observableObjects.TryAdd(key, obj))
            {
#if CC_DEBUG_ENABLED
                this.PrintWarning($"Object: {obj} with key: {key} already registered");
#endif
                return false;
            }

            return true;
        }

        public bool TryUnregisterObject(string key)
        {
            Guard.IsNotNull(key, nameof(key));

            return observableObjects.TryRemove(key, out _);
        }

        public bool TryUnregisterObject(object obj)
        {
            if (!TryResolveKey(obj, out var key))
                key = string.Empty;

            return TryUnregisterObject(key);
        }

        public bool IsObjectRegistered(string? key)
        {
            if (key is null)
                return false;

            return observableObjects.ContainsKey(key);
        }

        public bool IsObjectRegistered(object obj)
        {
            if (!TryResolveKey(obj, out var key))
                key = string.Empty;

            return IsObjectRegistered(key);
        }

        public bool TryResolveKey(
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

        public SaveData GetSaveData()
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

        public override string ToString()
        {
            return $"({nameof(Name)}: {Name}; {nameof(ID)}: {ID})";
        }

        private string ResolveGameObjectKey(GameObject go)
        {
            return go.GetExtraInfo().ToString();
        }

        private string ResolveComponentKey(Component cmp)
        {
            return cmp.GetExtraInfo().ToString();
        }

        private IEnumerable<(object obj, string key, Func<object, ISnapshot> converter)> GetObjectConverterPairs()
        {
            if (!observableObjects.IsEmpty)
                return Array.Empty<(object obj, string key, Func<object, ISnapshot> converter)>();

            return observableObjects.Select(
                static pair =>
                {
                    var objType = pair.Value.GetType();

                    return (pair.Value, pair.Key, SaveSystem.GetSnapshotConverter(objType));
                });
        }
    }
}
