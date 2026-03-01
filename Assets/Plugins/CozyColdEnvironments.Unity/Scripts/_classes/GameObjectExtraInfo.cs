using CCEnvs.Attributes.Serialization;
using CCEnvs.FuncLanguage;
using Newtonsoft.Json;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    [Serializable]
    [SerializationDescriptor("GameObjectExtraInfo", "3a2f26f8-31d8-4f71-831f-af7446d37f30")]
    public sealed class GameObjectExtraInfo
    {
        [field: SerializeField]
        [JsonProperty("persistentGUID")]
        public string? PersistenGuid { get; private set; }

        [field: SerializeField]
        [JsonProperty("hierarchyPath")]
        public HierarchyPath HierarchyPath { get; private set; }

        public GameObjectExtraInfo(GameObject gameObject)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));

            PersistenGuid = gameObject.GetPersistentGuid();
            HierarchyPath = gameObject.GetHierarchyPath();
        }

        [JsonConstructor]
        public GameObjectExtraInfo(string? persistenGuid, HierarchyPath hierarchyPath)
        {
            PersistenGuid = persistenGuid;
            HierarchyPath = hierarchyPath;
        }

        public Maybe<GameObject> FindGameObject(bool includeInactive = false)
        {
            if (PersistenGuid.IsNotNullOrEmpty()
                &&
                GameObjectHelper.FindByPersistenGuid(PersistenGuid, includeInactive).TryGetValue(out var go))
            {
                return go;
            }

            if (HierarchyPath != default
                &&
                GameObjectHelper.FindByHierarchyPath(HierarchyPath, includeInactive).TryGetValue(out go))
            {
                return go;
            }

            return Maybe<GameObject>.None;
        }

        public override string ToString()
        {
            return $"({nameof(PersistenGuid)}: {PersistenGuid}; {nameof(HierarchyPath)}: {HierarchyPath})";
        }
    }

    public static class GameObjectExtraInfoExtensions
    {
        public static GameObjectExtraInfo GetExtraInfo(this GameObject source)
        {
            return new GameObjectExtraInfo(source);
        }

        public static GameObjectExtraInfo GetExtraInfo(this Component source)
        {
            return new GameObjectExtraInfo(source.gameObject);
        }
    }
}
