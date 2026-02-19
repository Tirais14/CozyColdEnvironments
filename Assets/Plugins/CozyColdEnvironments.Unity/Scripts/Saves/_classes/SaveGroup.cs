using CCEnvs.Collections;
using CCEnvs.Linq;
using CCEnvs.Snapshots;
using CommunityToolkit.Diagnostics;
using ObservableCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public class SaveGroup : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly ObservableDictionary<string, object> observableObjects = new();

        public IReadOnlyObservableDictionary<string, object> ObservableObjects => observableObjects;

        public string Name { get; }

        public SaveGroupCatalog Catalog { get; }

        public SaveGroup(
            SaveGroupCatalog catalog,
            string? name = null
            )
        {
            Guard.IsNotNull(catalog, nameof(catalog));

            Name = name ?? string.Empty;
            Catalog = catalog;
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

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return observableObjects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString()
        {
            return $"({nameof(Name)}: {Name})";
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
