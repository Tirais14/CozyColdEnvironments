using CCEnvs.Attributes.Serialization;
using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Linq;
using CCEnvs.Snapshots;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using ObservableCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    [Serializable]
    [TypeSerializationDescriptor("Saves.SaveGroup", "617e5bef-3872-4fae-b0d4-8d42f0893231")]
    public sealed class SaveGroup : IEquatable<SaveGroup?>, IEnumerable<KeyValuePair<string, object>>
    {
        private readonly ObservableDictionary<string, object> observableObjects = new();

        private int? hashCode;

        [JsonIgnore]
        public IReadOnlyObservableDictionary<string, object> ObservableObjects => observableObjects;

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("catalog")]
        public SaveCatalog Catalog { get; }

        [JsonConstructor]
        public SaveGroup(
            SaveCatalog catalog,
            string? name = null
            )
        {
            Guard.IsNotNull(catalog, nameof(catalog));

            Name = name ?? string.Empty;
            Catalog = catalog;
        }

        public static bool operator ==(SaveGroup? left, SaveGroup? right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (left is null || right is null)
                return false;

            return left.Name == right.Name
                    &&
                    left.Catalog == right.Catalog;
        }

        public static bool operator !=(SaveGroup? left, SaveGroup? right)
        {
            return !(left == right);
        }

        public void RegisterObject(object obj, string? key = null)
        {
            CC.Guard.IsNotNull(obj, nameof(obj));

            if (key is null && !TryResolveKey(obj, out key))
                key = string.Empty;

            observableObjects.Add(key, obj);
        }

        public bool UnregisterObject(string key)
        {
            Guard.IsNotNull(key, nameof(key));

            return observableObjects.Remove(key);
        }

        public bool UnregisterObject(object obj)
        {
            if (!TryResolveKey(obj, out var key))
                key = string.Empty;

            return UnregisterObject(key);
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

        public ISnapshot GetObjectSnapshot(string? key)
        {
            key ??= string.Empty;

            var obj = observableObjects[key];

            var objType = obj.GetType();

            var converter = SaveSystem.Converters[objType];

            return converter(obj);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return observableObjects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString()
        {
            return $"({nameof(Name)}: {Name})";
        }

        public bool Equals(SaveGroup? other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is SaveGroup typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            hashCode ??= HashCode.Combine(Name, Catalog);

            return hashCode.Value;
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
            if (!observableObjects.IsEmpty())
                return Array.Empty<(object obj, string key, Func<object, ISnapshot> converter)>();

            return from obj in observableObjects
                   select (obj, objType: obj.GetType()) into objInfo
                   where SaveSystem.Converters.ContainsKey(objInfo.objType)
                   select (objInfo.obj.Value, objInfo.obj.Key, SaveSystem.Converters[objInfo.objType]);
        }
    }
}
