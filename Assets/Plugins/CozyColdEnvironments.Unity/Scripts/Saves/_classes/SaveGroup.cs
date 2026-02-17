using SuperLinq;
using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    [Serializable]
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

        public void RegisterObject(object obj, string? key = null)
        {
            CC.Guard.IsNotNull(obj, nameof(obj));

            if (key is null && !TryResolveKey(obj, out key))
                key = string.Empty;

            if (observableObjects.ContainsKey(key))
                throw new ArgumentException($"Object: {obj} with key: {key} already registered");

            observableObjects.TryAdd(key, obj);
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
            return SaveDataFactory.Create()
        }

        private string ResolveGameObjectKey(GameObject go)
        {
            return go.GetExtraInfo().ToString();
        }

        private string ResolveComponentKey(Component cmp)
        {
            return cmp.GetExtraInfo().ToString();
        }
    }
}
